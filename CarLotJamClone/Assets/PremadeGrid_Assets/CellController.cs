using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class CellController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterHandler _characterPrefab;
    [SerializeField] GameObject _obstaclePrefab;
    [SerializeField] CarController _carPrefab;
    [SerializeField] Renderer _renderer;
    public LayerMask carLayer;

    [Header("Materials")]
    [SerializeField] Material _defaultMaterial;
    [SerializeField] Material _selectedMaterial;
    [SerializeField] Material _failedMaterial;

    [Header("Debug")]
    [SerializeField] bool _isSelected;
    [SerializeField] Vector2 _coordinates = Vector2.zero;
    Vector3 halfExtents = new Vector3(.45f, .25f, .45f);
    CarController carController;
    bool nearByTheCar;
    Node _node;


    private void Start()
    {
        Pathfinding.instance.PathFoundEvent += OnPathFound;
        Pathfinding.instance.NoPathCanBeFoundEvent += OnNoPathCreated;

        if (Physics.CheckBox(transform.position, halfExtents, Quaternion.identity, carLayer))
        {
            _node.SetWalkable(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, halfExtents * 2);
    }

    public void Initialize(int x, int y, Vector3 _worldPos, Node node = null)
    {
        _coordinates.x = x;
        _coordinates.y = y;
        _node = node;
        transform.localPosition = _worldPos;
        gameObject.name = "Cell" + "(" + x + ", " + y + ")";
    }
    private void OnPathFound(List<CellController> obj)
    {
        if (!_isSelected) return;

        GetReleased();
        SetMaterial(_selectedMaterial);
    }
    private void OnNoPathCreated()
    {
        if (!_isSelected) return;

        GetReleased();

        IEnumerator PulseRoutine()
        {
            SetMaterial(_failedMaterial);
            yield return new WaitForSeconds(.25f);
            SetMaterial(_defaultMaterial);
        }

        StartCoroutine(PulseRoutine());
    }

    #region Spawning Region

    public void SpawnedWithCharacter(ColorPreferences colorPrefs)
    {
        CharacterHandler characterHandler = Instantiate(_characterPrefab, transform.position, Quaternion.identity);
        characterHandler.Initialize(colorPrefs, this);
    }

    public void SpawnedAsObstacle()
    {
        Instantiate(_obstaclePrefab, transform);
        _node.walkable = false;
    }

    public void SpawnWithCar(ColorPreferences colorPrefs, LookDirection lookDirection)
    {
        CarController carClone = Instantiate(_carPrefab, transform.position, Quaternion.identity);
        carClone.Initialize(colorPrefs, this, lookDirection);
        _node.walkable = false;
    }

    #endregion
    #region Getters & Setters
    public void SetMaterial(Material mat = null)
    {
        if (mat == null) mat = _defaultMaterial;

        _renderer.sharedMaterial = mat;
    }
    public Vector2 GetCoordinates()
    {
        return _coordinates;
    }

    public void GetSelected()
    {
        _isSelected = true;
    }

    public void GetReleased()
    {
        _isSelected = false;
    }

    public Node GetNode()
    {
        return _node;
    }

    public void SetNode(Node node)
    {
        _node = node;
    }
    public bool CheckCellNearByCar()
    {
        return nearByTheCar;
    }

    public void SetCellCarInteraction(bool isNearByCar, CarController car)
    {
        carController = car;
        nearByTheCar = isNearByCar;
    }

    public CarController GetNearCar()
    {
        return carController;
    }

    #endregion
}
