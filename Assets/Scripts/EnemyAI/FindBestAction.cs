using AI.Data;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AI.Evaluation {
    public class FindBestAction {

        UnitController thisUnit;
        AIPersonality personality;
        Pathfinder.TileInfo tileInfo;
        AttackManager attackManager;


        public FindBestAction(UnitController thisUnit, AIPersonality personality, Pathfinder.TileInfo tileInfo, AttackManager attackManager) {
            this.thisUnit = thisUnit;
            this.personality = personality;
            this.tileInfo = tileInfo;
            this.attackManager = attackManager;
        }

        public TurnAction EvaluateBestActions(List<UnitController> playerUnits) {
            TurnAction bestTurnAction = new TurnAction(TurnActionType.None);

            // evaluate normal attack value:
            for(int i = 0; i < playerUnits.Count; i++) {

                int targetValue = EvaluateHitChance(playerUnits[i]);

                targetValue += EvaluateTargetValue(playerUnits[i]);
                


                if (targetValue > bestTurnAction.actionValue) {
                    bestTurnAction.actionValue = targetValue;
                    bestTurnAction.type = TurnActionType.Attack;
                    bestTurnAction.target = playerUnits[i];
                    bestTurnAction.cost = thisUnit.attackCost;
                }
            }

            return bestTurnAction;
        }

        private int EvaluateHitChance(UnitController playerUnit) {
            int hitChance = attackManager.CheckChanceToHitFromTileCoords(tileInfo.coords, playerUnit.transform, thisUnit);

            if(hitChance <= 0) { // no chance to hit
                return 0;
            }

            int value = hitChance / personality.hitChanceValueDividend;

            return value;
        }

        private int EvaluateTargetValue(UnitController playerUnit) {
            int value = 0;

            int targetHealth = playerUnit.healthManager.GetHealth();
            if(targetHealth == 0) { // target dead
                return 0;
            }

            value =  Mathf.RoundToInt((-1f * Mathf.Log(targetHealth) + personality.hpValueMin) * personality.hpValueMultiplier);

            return value;
        }
    } 
}
