using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UIElements;
using System.Collections;

public class AttackManager : MonoBehaviour
{

    System.Random random;

    CastAttack castAttack;

    [SerializeField] private UnityEvent selectedAttack;
    [SerializeField] private UnityEvent deselectedAttack;
    [SerializeField] private UnityEvent<UnitController> Target;

    public void SelectAttack() => selectedAttack.Invoke();
    public void DeselectAttack() => deselectedAttack.Invoke();

    public bool attacking = false;

    private void Start() {
        castAttack = GetComponent<CastAttack>();
        random = new System.Random();

    }

    public void OnAttackToggle(bool isSelected) {
        Debug.Log("Toggle fired with value: " + isSelected);

        if (isSelected) {
            attacking = true;
            SelectAttack();
        }
        else {
            attacking = false;
            DeselectAttack();
        }
    }

    public void Targeted(UnitController unit) {
        Target.Invoke(unit);
    }

    public void AddTargetListener(UnityAction<UnitController> listener) {
        Target.AddListener(listener);
    }

    public void RemoveTargetListener(UnityAction<UnitController> listener)
    {
        Target.RemoveListener(listener);
    }

    public IEnumerator Attack(UnitController attackingUnit, UnitController targetUnit) {
        attackingUnit.SetState(UnitController.UnitState.Aiming);
        yield return new WaitForSeconds(1f); // Simulate aiming time
        int randomNumber = random.Next(1, 101);

        int hitChance = CheckChanceToHitFromUnit(attackingUnit, targetUnit);

        if (randomNumber > hitChance) {
            Debug.Log("Attack missed! Number rolled: " + randomNumber + ", Hit chance: " + hitChance);

        }

        else {
            Debug.Log("Attack Hit! Number rolled: " + randomNumber + ", Hit chance: " + hitChance);

            attackingUnit.unitAttack.sendAttack(targetUnit);
        }
        attackingUnit.SetState(UnitController.UnitState.Idle);
    }
    public int CheckChanceToHitFromUnit(UnitController attackingUnit, UnitController targetUnit) {
        Vector3 attackingUnitVector3 = attackingUnit.transform.position;

        int hitChance = CheckAttack(attackingUnitVector3, targetUnit.transform.position, attackingUnit);

        if (hitChance == -1) {
            Debug.Log("No ray can hit!");
        }
        
        return hitChance;
    }

    public int CheckChanceToHitFromTileCoords(Vector2Int tile, Transform targetUnit, UnitController attacker) {
        Vector3 tileWorldPos = GameContext.Instance.GridManager.GridToWorld(tile);

        int hitChance = CheckAttack(tileWorldPos, targetUnit.transform.position, attacker);

        if (hitChance == -1) {
            Debug.Log("No ray can hit!");
        }

        return hitChance;
    }

    private int CheckAttack(Vector3 attackingUnitPos, Vector3 targetUnit, UnitController attacker) {
        Rays[] attackingRays = new Rays[3];
        Rays bestRay = new Rays();

        int bestHitChance = -1;

        attackingRays = castAttack.Cast3Rays(attackingUnitPos, targetUnit);

        for (int i = 0; i < attackingRays.Length; i++) {
            if (attackingRays[i].HasLOS == false) continue;

            int currentRayChanceToHit = chanceToHit(attackingRays[i], attacker);

            if (currentRayChanceToHit > bestHitChance) {
                bestRay = attackingRays[i];
                bestHitChance = currentRayChanceToHit;

                continue;
            }
        }


        return bestHitChance;
    }
    private int chanceToHit(Rays ray, UnitController attacker) {
        int hitChance = 100;
        Debug.Log(ray.Cover);

        hitChance -= (int)(ray.rayDistance * attacker.weaponStats.rangeDropoffSpeed);
        Debug.Log("Hit chance after range dropoff: " + hitChance);

        if (ray.Cover==CoverType.Half) {
            hitChance = hitChance / 2;
        }
        hitChance = Mathf.Clamp(hitChance, 0, 100);

        Debug.Log(hitChance);
        return hitChance;
    }

    public int CheckCover(Vector3 shooter, Vector2Int target) {
        Vector3 end = GameContext.Instance.GridManager.GridToWorld(target);

        Rays[] rays = castAttack.Cast3Rays(shooter, end);

        foreach(Rays ray in rays) {           
            if(ray.Cover == CoverType.Full) {
                return 0; // no LOS, full cover
            }

            if(ray.Cover == CoverType.Half) {
                return 1; // no LOS, half cover
            }

            if (ray.Cover == CoverType.None) {
                return 2; // has LOS
            }
        }

        return -1; // no valid ray
    }
}
