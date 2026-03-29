using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponStats", menuName = "Tactics/Weapon Stats")]
public class WeaponStats : ScriptableObject {

    public string weaponName;

    public int damage;
    public int attackCost;

    public int rangeDropoffSpeed;
}
