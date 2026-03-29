using UnityEngine;
namespace AI.Data {
    
    [CreateAssetMenu(fileName = "New AIPersonality", menuName = "AI/AIPersonality")]

    public class AIPersonality : ScriptableObject {
        [Header("Player Move Settings")]
        public int normalMoveValue;
        public int sprintValue;

        public int stayStillBonus;

        [Header("Player Prox Settings")]
        public int maxPlayerProxValue;
        public int preferredPlayerProx;
        public int preferenceSharpness;

        [Header("Player Cover Settings")]

        [Tooltip("Cover value per player taken cover from")]
        public int coverValue = 2;


        [Header("Attack Value Settings")]

        [Tooltip("Changes Valuation of Hit Chance for Target Selection; higher = hit chance matters less, lower = hit chance matters more")]
        public int hitChanceValueDividend = 15;

        [Tooltip("Changes Valuation of Enemy Hp for Target Selection; changes exponentially; also raises min value, change hpValueMin to reflect")]
        public int hpValueMultiplier = 2;

        public int hpValueMin = 7;
    }
}
