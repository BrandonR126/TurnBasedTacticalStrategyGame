using UnityEngine;


[CreateAssetMenu(fileName = "NewUnitStats", menuName = "Tactics/Unit Stats")]
public class UnitStats : ScriptableObject {
    public int mobility;

    public float sprintValue;
    public int sprintCost;

    public int sprintMobility =>
        Mathf.FloorToInt(mobility * sprintValue);

    public int maxHealth;

    public int actionPoints;
}
