using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Grid : MonoSingleton<Grid>
{
    [Header("Configuration")]
    [SerializeField] float CellXLength;
    [SerializeField] float CellYLength;
    public LayerMask unwalkableMask;
    public int gridSizeX, gridSizeY;

    [Header("References")]
    [SerializeField] GameObject _cellParentTr;
    [SerializeField] CellController _cellPrefab;

    [Header("Debug")]
    public List<Node> path;
    public List<CellController> cellControllers;
    Node[,] GridPlan;

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
                bool walkable = !(Physics.CheckSphere(worldPoint, .5f, unwalkableMask));

                CellController controller = Instantiate(_cellPrefab, Vector3.zero, _cellPrefab.transform.rotation, _cellParentTr.transform);
                controller.name = "CellController" + "(" + x + ", " + y + ")";
                controller.transform.localPosition = new Vector3(CellXLength * x, 0, -(CellYLength * y));

                GridPlan[x, y] = new Node(_walkable: walkable, _worldPos: worldPoint, x, y, _cell: controller);
                Node nodeClone = GridPlan[x, y];
                controller.SetCoordinatesAndNode(x, y, node: nodeClone);

                controller.transform.localPosition -= new Vector3(((desiredRowCount - 1) / 2f) * CellXLength, 0, -((desiredColumnCount - 1) / 2f) * CellYLength);
                cellControllers.Add(controller);
            }
        }
        _cellParentTr.transform.position = Vector3.zero;
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

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector2 gridWorldSize = new Vector2(gridSizeX * CellXLength, gridSizeY * CellYLength);

        Debug.Log("World size: " + gridWorldSize);
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return GridPlan[x, y];
    }

    public Node GetClosestNodeToWorldPos(Vector3 origin)
    {
        Node closestNode = new Node(false, Vector3.zero, 0, 0, null);
        Vector2 cloneOrigin = new Vector2(origin.x, origin.z);
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < cellControllers.Count; i++)
        {
            CellController cell = cellControllers[i];
            float distance = Vector2.Distance(cloneOrigin, cell.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                cell.SetColor(Color.red);
                closestNode = cell.GetNode();
            }
        }
        Debug.Log("Cell count = " + closestNode.cell.name);

        return closestNode;
    }


    private void DestroyPreviousGrid()
    {
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
        cellControllers.Clear();
    }
    public void SetPathMaterial(List<Node> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            CellController cell = path[i].cell;
            //cell.SetColor(Color.black);
            cell.gameObject.SetActive(false);
            cell.isPath = true;
        }
    }
}