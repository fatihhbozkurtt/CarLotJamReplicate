using System;
using System.Collections.Generic;
using UnityEngine;

public enum LookDirection
{
    Left,
    Right,
    Forward,
    Backward
}

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<CarsInnerInfo> meshes;

    [Header("Debug")]
    [SerializeField] LookDirection lookDir;
    [SerializeField] ColorPreferences carColor;
    [SerializeField] CellController originCell;
    [SerializeField] List<CellController> doorCells;
    CarsInnerInfo innerInfo;

    public void Initialize(ColorPreferences colorPreferences, CellController cell, LookDirection lookDirection)
    {
        originCell = cell;

        lookDir = lookDirection;
        SetAxis(lookDir);

        carColor = colorPreferences;
        SetCarMeshByColor(colorPreferences);

        Grid.instance.NewGridCreatedEvent += OnNewGridCreated;
    }

    void OnNewGridCreated()
    {
        doorCells = GetDoorsCells();
        for (int i = 0; i < doorCells.Count; i++)
        {
            doorCells[i].SetCellCarInteraction(isNearByCar: true, this);
        }

        GetComponent<Collider>().enabled = true;
    }

    void SetCarMeshByColor(ColorPreferences colorPreference)
    {
        int carIndex = 999;

        switch (colorPreference)
        {
            case ColorPreferences.Red:
                carIndex = 0;
                break;
            case ColorPreferences.Blue:
                carIndex = 1;
                break;
            case ColorPreferences.Green:
                carIndex = 2;
                break;
            default:
                break;
        }


        for (int i = 0; i < meshes.Count; i++)
        {
            if (i == carIndex)
            {
                innerInfo = meshes[i];
                meshes[i].gameObject.SetActive(true);
            }
            else
            {
                Destroy(meshes[i].gameObject);
            }
        }

        meshes.Clear();
    }
    public Vector3 GetSeatPos()
    {
        return innerInfo.SeatTransform.position;
    }

    void SetAxis(LookDirection carAxis)
    {
        switch (carAxis)
        {
            case LookDirection.Left:
                transform.Rotate(new Vector3(0, -90, 0));
                break;
            case LookDirection.Right:
                transform.Rotate(new Vector3(0, 90, 0));
                break;
            case LookDirection.Backward:
                transform.Rotate(new Vector3(0, 180, 0));
                break;
            default:
                break;
        }
    }

    List<CellController> GetDoorsCells()
    {
        float yRot = Mathf.Abs(transform.rotation.y);
        // bool horizontal = yRot == 90 ? false : true; // A sneaky way to understand the axis, I am not proud of it but it works effortlessly
        bool horizontal = false;


        if (lookDir == LookDirection.Forward || lookDir == LookDirection.Backward)
            horizontal = true;
        else
            horizontal = false;

        Debug.Log("Horzontal: " + horizontal + ", look dir: " + lookDir);
        List<CellController> doorCells = Grid.instance.GetNeighboursByAxis(originCell.GetCoordinates(), horizontal);

        for (int i = 0; i < doorCells.Count; i++)
        {
            Debug.Log("Door cells: " + doorCells.Count + ", " + doorCells[i].GetCoordinates());

        }
        return doorCells;
    }

    public CellController GetCell()
    {
        CellController targetCell = originCell;

        for (int i = 0; i < doorCells.Count; i++)
        {
            if (doorCells[i].GetNode().walkable)
            {
                targetCell = doorCells[i];
                break;
            }
        }
        return targetCell;
    }

    public ColorPreferences GetColor()
    {
        return carColor;
    }
}