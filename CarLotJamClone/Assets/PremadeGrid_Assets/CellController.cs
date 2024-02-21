using UnityEngine;

public class CellController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterHandler character;

    [Header("Debug")]
    [SerializeField] Vector2 _coordinates = Vector2.zero;
    [SerializeField] bool hasCharacter;
    Node _node;

    public void SpawnedWithCharacter(ColorPreferences colorPrefs)
    {
        CharacterHandler characterHandler = Instantiate(character, transform.position, Quaternion.identity);
        characterHandler.Initialize(colorPrefs, this);
        hasCharacter = true;  
    }

    public void SetCoordinatesAndNode(int x, int y, Vector3 _worldPos, Node node = null)
    {
        _coordinates.x = x;
        _coordinates.y = y;
        _node = node;
        transform.localPosition = _worldPos;
    }

    public void SetColor(Color color)
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = color;
    }

    //----
    public Vector2 GetCoordinates()
    {
        return _coordinates;
    }

    public void GetSelected(Color _color)
    {
        SetColor(_color);
       // _node.walkable = false;
    }

    public void GetReleased()
    {
        SetColor(Color.white);
      //  _node.walkable = true;
    }

    public Node GetNode()
    {
        return _node;
    }

}
