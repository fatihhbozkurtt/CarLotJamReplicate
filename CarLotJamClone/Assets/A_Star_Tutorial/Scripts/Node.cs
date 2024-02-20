
using System;
using UnityEngine;

[Serializable]
public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost; // dsitance from start point
    public int hCost; // distance to target point
    public Node parent;
    public CellController cell;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, CellController _cell)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        cell = _cell;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
