using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoSingleton<Grid>
{
    public event System.Action NewGridCreatedEvent;

    [Header("Configuration")]
    [SerializeField] float CellXLength;
    [SerializeField] float CellYLength;
    public int gridSizeX, gridSizeY;

    [Header("References")]
    [SerializeField] GameObject _cellParentTr;
    [SerializeField] CellController _cellPrefab;

    [Header("Debug")]
    public List<CellController> path;
    public List<CellController> cellControllers;
    Node[,] GridPlan;
    protected override void Awake()
    {
        base.Awake();

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        if (GridPlan != null)
        {
            DestroyPreviousGrid();
        }

        int desiredRowCount = gridSizeX;
        int desiredColumnCount = gridSizeY;
        GridPlan = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < desiredRowCount; x++)
        {
            for (int y = 0; y < desiredColumnCount; y++)
            {
                Vector3 worldPoint = new Vector3(CellXLength * x, 0, -(CellYLength * y));
                CellController cellController = Instantiate(_cellPrefab, Vector3.zero, _cellPrefab.transform.rotation, _cellParentTr.transform);
                cellController.transform.localPosition = new Vector3(CellXLength * x, 0, -(CellYLength * y));

                GridPlan[x, y] = new Node(_walkable: true, _worldPos: worldPoint, x, y, _cell: cellController);
                Node nodeClone = GridPlan[x, y];

                Vector3 cellPos = cellController.transform.localPosition - new Vector3((desiredRowCount - 1) / 2f * CellXLength, 0, -((desiredColumnCount - 1) / 2f) * CellYLength);
                cellController.Initialize(x, y, cellPos, node: nodeClone);
                cellControllers.Add(cellController);


                #region Temporary Test Region

                if (x == 0 && y == 0) cellController.SpawnedWithCharacter(ColorPreferences.Blue);
                if (x == 4 && y == 3) cellController.SpawnedWithCharacter(ColorPreferences.Red);
                if (x == 0 && y == 5) cellController.SpawnedWithCharacter(ColorPreferences.Green);
                if (x > 2 && x < 6 && y == 1) cellController.SpawnedAsObstacle();

                if (y == 2)
                {
                    if (x == 2)
                    {
                        cellController.SpawnWithCar(ColorPreferences.Red, LookDirection.Forward);
                    }

                }

                if (y == 2)
                {
                    if (x == 4) cellController.SpawnWithCar(ColorPreferences.Green, LookDirection.Left);
                }

                if (y == 6)
                {
                    if (x == 3) cellController.SpawnWithCar(ColorPreferences.Blue, LookDirection.Right);
                }
                #endregion

            }
        }
        _cellParentTr.transform.position = Vector3.zero;

        NewGridCreatedEvent?.Invoke();
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        int x = node.gridX;
        int y = node.gridY;

        if (x > 0)
        {
            if (GridPlan[x - 1, y].walkable)
            {
                neighbours.Add(GridPlan[x - 1, y]);
            }
        }
        if (x < gridSizeX - 1)
        {
            if (GridPlan[x + 1, y].walkable)
            {
                neighbours.Add(GridPlan[x + 1, y]);
            }
        }

        if (y > 0)
        {
            if (GridPlan[x, y - 1].walkable)
            {
                neighbours.Add(GridPlan[x, y - 1]);
            }
        }
        if (y < gridSizeY - 1)
        {
            if (GridPlan[x, y + 1].walkable)
            {
                neighbours.Add(GridPlan[x, y + 1]);
            }
        }
        return neighbours;
    }
    public List<CellController> GetNeighboursByAxis(Vector2 originCoordinates, bool horizontalAxis)
    {
        List<CellController> neighbours = new List<CellController>();
        CellController firstNeighbour;
        CellController secondNeighour;

        if (horizontalAxis)
        {
            firstNeighbour = GridPlan[(int)originCoordinates.x + 1, (int)originCoordinates.y].cell;
            secondNeighour = GridPlan[(int)originCoordinates.x - 1, (int)originCoordinates.y].cell;
        }
        else
        {
            firstNeighbour = GridPlan[(int)originCoordinates.x, (int)originCoordinates.y + 1].cell;
            secondNeighour = GridPlan[(int)originCoordinates.x, (int)originCoordinates.y - 1].cell;
        }

        if (firstNeighbour != null) neighbours.Add(firstNeighbour);
        if (secondNeighour != null) neighbours.Add(secondNeighour);

        return neighbours;
    }
    private void DestroyPreviousGrid()
    {
        cellControllers.Clear();
        for (int x = 0; x < GridPlan.GetLength(0); x++)
        {
            for (int y = 0; y < GridPlan.GetLength(1); y++)
            {
                DestroyImmediate(GridPlan[x, y].cell);
            }
        }

        foreach (Transform cellTr in _cellParentTr.transform)
        {
            DestroyImmediate(cellTr.gameObject);
        }
    }
}