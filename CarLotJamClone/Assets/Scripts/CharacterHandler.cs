using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ColorPreferences
{
    Red, Blue, Green
}

public class CharacterHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Renderer _renderer;
    [SerializeField] Material outlineMat;
    [SerializeField] Material[] _materials;

    [Header("Configuration")]
    [SerializeField] float moveSpeed;

    [Header("Debug")]
    [SerializeField] CellController currentCell;
    [SerializeField] CellController targetCell;
    [SerializeField] ColorPreferences _colorPreference;
    [SerializeField] List<Vector3> waypoints;
    PopUpCanvasController popUpCanvasController;
    int currentWaypointIndex = 0;
    bool moveToTargetCell;

    [Header("Test")]
    public bool outline;
    public bool removeOutline;

    private void Start()
    {
        Pathfinding.instance.PathFoundEvent += OnNewPathFound;
        popUpCanvasController = GetComponentInChildren<PopUpCanvasController>();
    }

    public void Initialize(ColorPreferences colorPreferences, CellController cell)
    {
        currentCell = cell;
        currentCell.GetNode().SetWalkable(false);

        SetCharacterColor(colorPreferences);
    }

    private void OnNewPathFound(List<CellController> cells)
    {
        if (InputManager.instance.GetSelectedCharacter() != this) return;

        for (int i = 0; i < cells.Count; i++)
        {
            Vector3 pos = cells[i].transform.position;
            waypoints.Add(pos);
        }

        Node currentNode = currentCell.GetNode();
        currentNode.SetWalkable(true);

        targetCell = cells[cells.Count - 1];
        Node targetNode = targetCell.GetNode();
        targetNode.SetWalkable(false);

        moveToTargetCell = true;

        popUpCanvasController.PopTheEmojiUp(isHappy: true);
    }

    private void Update()
    {
        if (outline)
        {
            SetOutline();
            outline = false;
        }
        if (removeOutline)
        {
            SetOutline(activate: false);
            removeOutline = false;
        }

        if (!moveToTargetCell) return;
        if (waypoints.Count == 0) return;

        if (currentWaypointIndex < waypoints.Count)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex];
            targetPosition.y = transform.position.y; // Keep the same Y position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            waypoints.Clear();
            moveToTargetCell = false;
            currentWaypointIndex = 0;

            //---- Cell Adjustments

            currentCell = targetCell;
            targetCell.SetMaterial();

            if (targetCell.CheckCellNearByCar())
                HopInTheCar(targetCell.GetNearCar());

            targetCell = null;
        }
    }

    void HopInTheCar(CarController targetCar)
    {
        if (targetCar.GetColor() != _colorPreference) return;
        if (targetCar == null) return;
        Vector3 seatPos = targetCar.GetSeatPos();

        transform.DOMove(seatPos, .5f);
        transform.DOScale(Vector3.zero, .5f).OnComplete(() =>
        {
            Node currentNode = currentCell.GetNode();
            currentNode.SetWalkable(true);

            Debug.Log("Character is in the car: " + _colorPreference);
        });

    }



    #region Getters & Setters
    void SetOutline(bool activate = true)
    {
        if (!activate)
        {
            Material[] currentMaterials = _renderer.materials;
            List<Material> newMaterialsList = new List<Material>(currentMaterials); // Convert to a list for easier manipulation

            newMaterialsList.RemoveAt(newMaterialsList.Count - 1);
            _renderer.materials = newMaterialsList.ToArray();
        }
        else
        {

            Material[] currentMaterials = _renderer.materials;
            Material[] newMaterials = new Material[currentMaterials.Length + 1];

            for (int i = 0; i < currentMaterials.Length; i++)
            {
                newMaterials[i] = currentMaterials[i];
            }

            Material newMat = outlineMat;
            newMaterials[currentMaterials.Length] = newMat;

            _renderer.materials = newMaterials;
        }

    }
    void SetCharacterColor(ColorPreferences colorPreference)
    {
        _colorPreference = colorPreference;
        switch (colorPreference)
        {
            case ColorPreferences.Red:
                _renderer.sharedMaterial = _materials[0];
                break;
            case ColorPreferences.Blue:
                _renderer.sharedMaterial = _materials[1];
                break;
            case ColorPreferences.Green:
                _renderer.sharedMaterial = _materials[2];
                break;
            default:
                break;
        }
    }
    public CellController GetCharCurrentCell()
    {
        return currentCell;
    }
    public ColorPreferences GetCharacterColor()
    {
        return _colorPreference;
    }
    #endregion
}
