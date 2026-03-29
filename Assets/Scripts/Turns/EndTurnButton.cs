using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public GameObject endTurnButton;

    TurnManager turnManager;

    private void Start()
    {
        turnManager = TurnManager.Instance;


        turnManager.OnPlayerTurnStarted += HandleTurnStarted;
        turnManager.OnPlayerTurnEnded += HandleTurnEnded;
    }

    private void OnDestroy()
    {
        // unsubscribe when object is destroyed
        if (turnManager != null)
            turnManager.OnPlayerTurnStarted -= HandleTurnStarted;
    }

    private void HandleTurnStarted()
    {
        gameObject.SetActive(true);
    }

    private void HandleTurnEnded()
    {
        gameObject.SetActive(false);
    }

    public void EndTurn()
    {
        turnManager.RequestEndTurn();
    }
}
