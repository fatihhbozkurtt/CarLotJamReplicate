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
    [SerializeField] Color _selectedCellColor;

    [Header("Debug")]
    [SerializeField] CharacterHandler selectedCaharacter;
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
                        selectedCaharacter = character;
                    }

                    if (selectedCaharacter == null) return;

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
                    _touchedCell.GetSelected(_selectedCellColor);
                    OnNewCellSelected();
                }
            }

            isTouchedDown = false;
            _touchedCell = null;
        }
    }

    void OnNewCellSelected()
    {
        if (selectedCaharacter == null) return;

        CellController startCell = selectedCaharacter.GetCharCurrentCell();
        CellController targetCell = _touchedCell;

        NewCellSelectedEvent?.Invoke(startCell, targetCell);
        selectedCaharacter = null;
    }

    public CharacterHandler GetSelectedCharacter()
    {
        return selectedCaharacter;
    }

}