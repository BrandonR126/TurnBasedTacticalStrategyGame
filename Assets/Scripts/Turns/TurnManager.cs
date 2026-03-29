using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{   
    public static TurnManager Instance;

    public TurnState CurrentTurn {  get; private set; }

    public event Action OnPlayerTurnStarted;
    public event Action OnPlayerTurnEnded;

    public event Action OnEnemyTurnStarted;
    public event Action OnEnemyTurnEnded;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        StartCombat();
    }
    public void StartCombat() {
        CurrentTurn = TurnState.Player;

        StartTurn();
    }

    private void StartTurn() {
        switch(CurrentTurn) {
            case TurnState.Player:
                Debug.Log("Player Turn Started");

                StartPlayerTurn();
                break;
            case TurnState.Enemy:
                Debug.Log("Enemy Turn Started");

                StartEnemyTurn();
                break;
        }
    }

    public void RequestEndTurn() {
        // future checks to ensure can end turn goes here
        
        
        EndTurn();
    }

    private void EndTurn() {
        if(CurrentTurn == TurnState.Player) {
            OnPlayerTurnEnded?.Invoke();

            CurrentTurn = TurnState.Enemy;
        }
        else if(CurrentTurn == TurnState.Enemy) {
            OnEnemyTurnEnded?.Invoke();

            CurrentTurn = TurnState.Player;
        }

            StartTurn();
    }

    private void StartPlayerTurn() {
        OnPlayerTurnStarted?.Invoke();
    }

    private void StartEnemyTurn() {
        OnEnemyTurnStarted?.Invoke();
    }
}
