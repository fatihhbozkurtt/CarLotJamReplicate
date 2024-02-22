using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    #region Events

    public event System.Action TouchStartEvent;
    public event System.Action TouchEndEvent;
    public void OnPointerDown()
    {
        TouchStartEvent?.Invoke();
    }

    public void OnPointerUp()
    {
        TouchEndEvent?.Invoke();
    }
    #endregion
    public delegate void NewCellSelectedEventDelegate(CellController _startCell, CellController _targetCell);
    public event NewCellSelectedEventDelegate NewCellSelectedEvent;

    [Header("Configuration")]
    [SerializeField] Material _selectedCellMaterial;

    [Header("Debug")]
    [SerializeField] CharacterHandler selectedCharacter;
    private bool isTouchedDown = false;
    private CellController _touchedCell = null;


    public void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!isTouchedDown)
            {
                isTouchedDown = true;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.TryGetComponent(out CharacterHandler character))
                    {
                        selectedCharacter = character;
                    }

                    if (selectedCharacter == null) return;

                    if (hit.collider.TryGetComponent(out CellController cell))
                    {
                        _touchedCell = cell;
                    }
                }
            }
        }
        else
        {
            if (isTouchedDown && _touchedCell)
            { 
                Node node = _touchedCell.GetNode();

                if (node.walkable)
                {
                    _touchedCell.GetSelected();
                    OnNewCellSelected();
                }
            }

            isTouchedDown = false;
            _touchedCell = null;
        }
    }

    void OnNewCellSelected()
    {
        if (selectedCharacter == null) return;

        CellController startCell = selectedCharacter.GetCharCurrentCell();
        CellController targetCell = _touchedCell;

        NewCellSelectedEvent?.Invoke(startCell, targetCell);
        selectedCharacter = null;
    }

    public CharacterHandler GetSelectedCharacter()
    {
        return selectedCharacter;
    }

}