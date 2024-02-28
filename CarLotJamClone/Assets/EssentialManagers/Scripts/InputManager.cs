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

    public event System.Action<CharacterHandler> NewCharacterSelected;
    public event System.Action NoWalkableNodesNearCarEvent;

    [Header("Configuration")]
    [SerializeField] Material _selectedCellMaterial;

    [Header("Debug")]
    [SerializeField] CharacterHandler selectedCharacter;
    [SerializeField] CarController _selectedCar;
    [SerializeField] CellController _touchedCell;
    private bool isTouchedDown = false;


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
                        NewCharacterSelected?.Invoke(selectedCharacter);
                    }

                    if (selectedCharacter == null) return;

                    if (hit.collider.TryGetComponent(out CarController car))
                    {
                        if (CheckIfColorsMatch(car))
                        {
                            _touchedCell = car.GetCell();
                            _selectedCar = car;
                        }
                        else
                            NoWalkableNodesNearCarEvent?.Invoke();

                    }
                    else if (hit.collider.TryGetComponent(out CellController cell))
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
                else
                {
                    if (_selectedCar) NoWalkableNodesNearCarEvent?.Invoke();
                }
            }

            isTouchedDown = false;
            _touchedCell = null;
            _selectedCar = null;
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

    bool CheckIfColorsMatch(CarController carColor)
    {
        return selectedCharacter.GetCharacterColor() == carColor.GetColor();
    }
}