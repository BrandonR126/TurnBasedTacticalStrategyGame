using System.Collections.Generic;
using UnityEngine;
using AI.Data;

namespace AI.Evaluation {
    public class FindBestTile {
        AIPersonality personality;
        Transform selfTransform;
        List<UnitController> playerUnits;
        AttackManager attackManager;


        private LayerMask worldMask;
        private LayerMask coverMask;

        public FindBestTile(AIPersonality personality, Transform selfTransform, List<UnitController> playerUnits, AttackManager attackManager) {
            this.personality = personality;
            this.selfTransform = selfTransform;
            this.playerUnits = playerUnits;
            this.attackManager = attackManager;
        }

        public int EvaluateTileValue(Pathfinder.TileInfo tile, List<UnitController> playerUnits) {
            int value = 0;

            var proxResult = PlayerProximityValue(tile.coords, playerUnits); // run first - finds closter player prox val
            value += proxResult.value;

            value += MovementTypeValue(tile);

            value += CoverValue(tile);

            if (tile.coords == GameContext.Instance.GridManager.WorldToGrid(selfTransform.position)) {
                value += personality.stayStillBonus;
            }

            return value;
        }

        // Determines the value of a tile based on whether it's reachable or sprintable.
        private int MovementTypeValue(Pathfinder.TileInfo info) {

            if (info.reachable) {
                return personality.normalMoveValue;
            }

            if (info.sprintable) {
                return personality.sprintValue;
            }

            return 0;
        }

        // Determines the value of a tile based on its proximity to player units.
        private (int value, int distance) PlayerProximityValue(Vector2Int tilePos, List<UnitController> playerUnits) {
            int closestDistance = int.MaxValue;

            foreach (var player in playerUnits) {
                Vector2Int playerPos = GameContext.Instance.GridManager.WorldToGrid(player.transform.position);

                int dx = Mathf.Abs(playerPos.x - tilePos.x);
                int dy = Mathf.Abs(playerPos.y - tilePos.y);
                int distance = Mathf.Max(dx, dy);

                closestDistance = Mathf.Min(closestDistance, distance);
            }

            if (closestDistance == int.MaxValue)
                return (0, int.MaxValue);

            int preferredDistance = Mathf.Abs(personality.preferredPlayerProx - closestDistance);
            int value = ProxFalloff(preferredDistance);

            return (value, closestDistance);
        }


        private int ProxFalloff(int distanceFromPreferred) {
            float sigma = personality.preferenceSharpness;
            float gaussian = Mathf.Exp(-(distanceFromPreferred * distanceFromPreferred) / (2 * sigma * sigma));

            return Mathf.RoundToInt(gaussian * personality.maxPlayerProxValue);
        }

        // Determines value based off amount of cover from players
        private int CoverValue(Pathfinder.TileInfo info) {
            int coverValue = 0;

            foreach (UnitController player in playerUnits) {
                int coverHit = attackManager.CheckCover(player.transform.position, info.coords);

                if (coverHit == 0) {
                    coverValue += personality.coverValue;
                }
                else if (coverHit == 1) {
                    coverValue += personality.coverValue / 2;
                }

                else {
                    coverValue += 0;
                }
            }

            return coverValue;
        }
    }
}
