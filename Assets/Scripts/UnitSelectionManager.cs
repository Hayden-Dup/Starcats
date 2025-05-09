using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance;

    private List<SelectableUnit> selectedUnits = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DeselectAllUnits();
        }

        if (Input.GetMouseButtonDown(0)) // Left Click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                SelectableUnit unit = hit.collider.GetComponent<SelectableUnit>();
                if (unit != null && !selectedUnits.Contains(unit))
                {
                    selectedUnits.Add(unit);
                    unit.SetSelected(true);
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right Click
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var unit in selectedUnits)
            {
                unit.MoveTo(target);
            }
        }
    }

    private void DeselectAllUnits()
    {
        foreach (var unit in selectedUnits)
        {
            unit.SetSelected(false);
        }
        selectedUnits.Clear();
    }
}
