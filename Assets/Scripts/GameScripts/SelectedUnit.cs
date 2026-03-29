using UnityEngine;

public class SelectedUnit : MonoBehaviour
{
    public UnitController selectedUnit;

    public void SetSelectedUnit(UnitController newUnit)
    {
        selectedUnit = newUnit;
    }

    public void DeselectUnit() {
        selectedUnit = null;
    }
}
