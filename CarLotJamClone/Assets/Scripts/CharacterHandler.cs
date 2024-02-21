using System.Collections.Generic;
using UnityEngine;

public enum ColorPreferences
{
    Red, Blue, Green
}

public class CharacterHandler : MonoBehaviour
{


    [Header("Configuration")]
    [SerializeField] float moveSpeed;

    [Header("Debug")]
    [SerializeField] CellController currentCell;
    [SerializeField] CellController targetCell;
    public List<Vector3> waypoints = new List<Vector3>();
    private int currentWaypointIndex = 0;
    Node currentNode;
    bool moveToTargetCell;
    private void Start()
    {
        Pathfinding.instance.PathFoundEvent += OnNewPathFound;
    }

    public void Initialize(ColorPreferences colorPreferences, CellController cell)
    {
        currentCell = cell;
        currentNode = currentCell.GetNode();
        SetColorPreferences(colorPreferences);
    }

    private void OnNewPathFound(List<CellController> cells)
    {
        if (InputManager.instance.GetSelectedCharacter() != this) return;

        for (int i = 0; i < cells.Count; i++)
        {
            Vector3 pos = cells[i].transform.position;
            waypoints.Add(pos);
        }
        Debug.Log("Path found character is starting to move");

        currentCell.GetReleased();
        targetCell = cells[cells.Count - 1];
        //TODO: ayný cell i target almalarýný önleme amaçlý
        // targetCell.GetNode().walkable = false;
        moveToTargetCell = true;
    }

    private void Update()
    {
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
            targetCell.SetColor(Color.white);

            targetCell = null;
        }
    }

    #region Getters & Setters

    public CellController GetCharCurrentCell()
    {
        return currentCell;
    }

    void SetColorPreferences(ColorPreferences colorPreference)
    {
        Renderer renderer = GetComponentInChildren<Renderer>();

        switch (colorPreference)
        {
            case ColorPreferences.Red:
                renderer.material.color = Color.red;
                break;
            case ColorPreferences.Blue:
                renderer.material.color = Color.blue;
                break;
            case ColorPreferences.Green:
                renderer.material.color = Color.green;
                break;
            default:
                break;
        }

    }
    #endregion
}
