using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoSingleton<Pathfinding>
{
    public event System.Action<List<CellController>> PathFoundEvent;
    public event System.Action NoPathCanBeFoundEvent;

    Grid grid;

    private void Start()
    {
        InputManager.instance.NewCellSelectedEvent += FindPath;
        grid = Grid.instance;
    }
    void FindPath(CellController startCell, CellController targetCell)
    {
        Node startNode = startCell.GetNode();
        Node targetNode = targetCell.GetNode();

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        bool pathFound = false; // Flag to track if a path has been found

        while (openSet.Count > 0)
        {
            Node firstNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < firstNode.fCost || openSet[i].fCost == firstNode.fCost)
                {
                    if (openSet[i].hCost < firstNode.hCost)
                        firstNode = openSet[i];
                }
            }

            openSet.Remove(firstNode);
            closedSet.Add(firstNode);

            if (firstNode == targetNode)
            {
                Debug.Log("A clear path is found");
                RetracePath(startNode, targetNode);
                pathFound = true;
                break; // Exit the loop if the target firstNode is found
            }

            foreach (Node neighbour in grid.GetNeighbours(firstNode))
            {
                if (neighbour == firstNode) continue;
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = firstNode.gCost + GetDistance(firstNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = firstNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        if (!pathFound)
        {
            Debug.Log("No path found!");
            NoPathCanBeFoundEvent?.Invoke();
            return;
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<CellController> path = new List<CellController>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.cell);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        PathFoundEvent?.Invoke(path);
        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
/*  void FindPath(CellController startCell, CellController targetCell)
  {
      Node startNode = startCell.GetNode();
      Node targetNode = targetCell.GetNode();

      List<Node> openSet = new List<Node>();
      HashSet<Node> closedSet = new HashSet<Node>();
      openSet.Add(startNode);


      while (openSet.Count > 0)
      {
          Node firstNode = openSet[0];
          for (int i = 1; i < openSet.Count; i++)
          {
              if (openSet[i].fCost < firstNode.fCost || openSet[i].fCost == firstNode.fCost)
              {
                  if (openSet[i].hCost < firstNode.hCost)
                      firstNode = openSet[i];
              }
          }

          openSet.Remove(firstNode);
          closedSet.Add(firstNode);

          if (firstNode == targetNode)
          {
              Debug.Log("A clear path is found");
              RetracePath(startNode, targetNode);
              return;
          }

          foreach (Node neighbour in grid.GetNeighbours(firstNode))
          {
              if (!neighbour.walkable || closedSet.Contains(neighbour))
              {
                  continue;
              }

              int newCostToNeighbour = firstNode.gCost + GetDistance(firstNode, neighbour);
              if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
              {
                  neighbour.gCost = newCostToNeighbour;
                  neighbour.hCost = GetDistance(neighbour, targetNode);
                  neighbour.parent = firstNode;

                  if (!openSet.Contains(neighbour))
                      openSet.Add(neighbour);
              }
          }
      }
  }*/