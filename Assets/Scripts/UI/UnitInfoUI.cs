using UnityEngine;
using TMPro;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unitClassText;
    [SerializeField] TextMeshProUGUI unitWeaponText;
    [SerializeField] TextMeshProUGUI unitHealthText;
    [SerializeField] GameObject UnitInfo;

    public void ShowUnitInfo(UnitController hoveredUnit) {
        unitClassText.text = $"Unit Class: {hoveredUnit.baseStats.name}"; 
        unitWeaponText.text = $"Weapon: {hoveredUnit.weaponStats.weaponName}";
        unitHealthText.text = $"Health: {hoveredUnit.healthManager.GetHealth()}/{hoveredUnit.baseStats.maxHealth}";

        UnitInfo.SetActive(true);
    }

    public void HideUnitInfo() {
        UnitInfo.SetActive(false);
    }
}
