using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public class UnitController : MonoBehaviour, IClickable, ISelectable {
   
    Animator animator;
    [HideInInspector] public HealthManager healthManager;
    [HideInInspector] public UnitAttack unitAttack;
    HandleSelected handleSelected;
    TeamController teamController;

    public UnitStats baseStats;
    public Team team;

    public WeaponStats weaponStats;

    public enum UnitState { Idle, Walking, Sprinting, Aiming}
    public UnitState currentState = UnitState.Idle;

    public bool isSelected = false;

    [Header("Unit Stats")]
    private int baseHealth;
    private int baseMobility;
    private int baseUnitAp;
    private int baseSprint;
    public int currentMobility;
    public float sprintAmount;
    public int sprintCost;
    public int unitAP;

    [Header("Weapon Stats")]
    private int baseAttackDamage;
    private int baseFireCost;
    public int attackDamage;
    public int attackCost;

    [Header("Team Stats")]
    public int teamAP;

    public bool canSprint => 
        sprintAmount > 0;

    private void Awake() {
        healthManager = GetComponent<HealthManager>();
        unitAttack = GetComponent<UnitAttack>();
        teamController = GetComponentInParent<TeamController>();

        if (baseStats != null) {
            baseMobility = baseStats.mobility;
            baseSprint = baseStats.sprintMobility;
            baseHealth = baseStats.maxHealth;
            baseUnitAp = baseStats.actionPoints;
            sprintCost = baseStats.sprintCost;

            healthManager.maxHealth = baseHealth;
        }

        else {
            Debug.LogError("Could not access base stats");
        }

        if (weaponStats != null) {
            baseAttackDamage = weaponStats.damage;
            baseFireCost = weaponStats.attackCost;
        }

        else {
            Debug.LogError("Could not access weapon stats");
        }

        teamController.RegisterUnit(this);

    }

    private void Start() {
        animator = GetComponentInChildren<Animator>();
        handleSelected = GetComponent<HandleSelected>();

        healthManager.onDeath.AddListener(OnDeath);
    }

    public void Selected() { 


        handleSelected.Selected();


    }
    
    public void Deselected() { 
    handleSelected.Deselected();
    }
    public void StartUnitTurn() {
        currentMobility = baseMobility;
        sprintAmount = baseSprint;
        unitAP = baseUnitAp;

        attackDamage = baseAttackDamage;
        attackCost = baseFireCost;
    }

    private void OnDeath() {
        teamController.UnregisterUnit(this);
        Destroy(gameObject);
    }

    public void OnClick() {
        if (UnitMovement.unitMoving) return;

        if (ClickManager.Instance == null) {
            Debug.LogError("ClickManager.Instance is NULL");
            return;
        }

        ClickManager.Instance.Select(this);
    }
    

    #region handle States/Animation
    public void SetState(UnitState newState) {
        Debug.Log($"Changing state from {currentState} to {newState}");
        if (currentState == newState) return;

        currentState = newState;

        animator.SetBool("IsIdle", newState == UnitState.Idle);
        animator.SetBool("IsWalking", newState == UnitState.Walking);
        animator.SetBool("IsSprinting", newState == UnitState.Sprinting);
        animator.SetBool("IsAiming", newState == UnitState.Aiming);
    }

    #endregion

}
