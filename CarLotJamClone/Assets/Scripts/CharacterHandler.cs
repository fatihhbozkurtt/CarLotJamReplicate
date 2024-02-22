using System.Collections.Generic;
using UnityEngine;

public enum ColorPreferences
{
    Red, Blue, Green
}

public class CharacterHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Renderer _renderer;
    [SerializeField] Material[] _materials;

    [Header("Configuration")]
    [SerializeField] float moveSpeed;

    [Header("Debug")]
    [SerializeField] CellController currentCell;
    [SerializeField] CellController targetCell;
    public List<Vector3> waypoints = new List<Vector3>();
    private int currentWaypointIndex = 0;
    bool moveToTargetCell;

    private void Start()
    {
        Pathfinding.instance.PathFoundEvent += OnNewPathFound;
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

        Debug.Log("Path found _characterPrefab is starting to move");

        Node currentNode = currentCell.GetNode();
        currentNode.SetWalkable(true);
        
        targetCell = cells[cells.Count - 1];
        Node targetNode = targetCell.GetNode();
        targetNode.SetWalkable(false);

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
            targetCell.SetMaterial();
           // targetCell.GetNode().SetWalkable(true);

            targetCell = null;
        }
    }

    #region Getters & Setters

    public CellController GetCharCurrentCell()
    {
        return currentCell;
    }

    void SetCharacterColor(ColorPreferences colorPreference)
    {
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
    #endregion
}
