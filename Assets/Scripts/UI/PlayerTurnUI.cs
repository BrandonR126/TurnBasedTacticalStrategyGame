using UnityEngine;
using TMPro;

public class PlayerTurnUI : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] GameObject playerTurnUI;
    [SerializeField] TextMeshProUGUI teamApUi;

    public void EnablePlayerUI() {
        playerTurnUI.SetActive(true);

        Debug.Log("Show Player Turn UI");
    }

    public void DisablePlayerUI() {
        playerTurnUI.SetActive(false);
        Debug.Log("Hide Player Turn UI");
    }

    public void UpdateApUI(int currentAP, int maxAP) {
        teamApUi.text = $"Team AP: {currentAP}/{maxAP}";
    }
}
