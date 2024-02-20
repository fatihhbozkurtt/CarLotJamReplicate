using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    [Header("References")]
    public GameObject crossImage;

    [Header("Debug")]
    [SerializeField] Vector2 _coordinates = Vector2.zero;
    public bool isPath;
    Node _node;
    public Vector2 GetCoordinates()
    {
        return _coordinates;
    }

    public void SetCoordinatesAndNode(int x, int y, Node node = null)
    {
        _coordinates.x = x;
        _coordinates.y = y;
        _node = node;
    }
    public void SetCrossImage(bool activate)
    {
        crossImage.SetActive(activate);
    }
    public Node GetNode()
    {
        return _node;
    }

    public void SetColor(Color color)
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = color;
    }
}
