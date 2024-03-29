
using UnityEngine;
public class PremadeInputManager : MonoBehaviour
{
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
                CellStatsContainer cellStats = GridManager.instance.GetCellStats(_touchedCell);
                if (!cellStats.IsOccupied)
                {
                    cellStats.IsOccupied = true;
                    // _touchedCell.SetCrossImage(activate: true);
                    // GridManager.instance.CheckAndDestroyMatches();
                }
            }

            isTouchedDown = false;
            _touchedCell = null;
        }
    }
}