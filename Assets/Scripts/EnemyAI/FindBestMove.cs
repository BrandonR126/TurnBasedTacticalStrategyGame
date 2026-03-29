using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AI.Data;


namespace AI.Evaluation {
    public class FindBestMove : MonoBehaviour {
        Pathfinder pathfinder;
        UnitController unitController;
        [SerializeField] AIPersonality personality;
        FindBestTile findBestTile;
        FindBestAction findBestAction;
        AttackManager attackManager;
        GridManager gridManager;
        TeamController teamController;

        private int AP;

        private List<UnitController> playerUnits = new List<UnitController>();
        private UnitController[] allUnits;

        private void Start() {
            pathfinder = GameContext.Instance.Pathfinder;
            unitController = GetComponent<UnitController>();
            attackManager = GameContext.Instance.AttackManager;
            gridManager = GameContext.Instance.GridManager;
            teamController = GetComponentInParent<TeamController>();

            allUnits = FindObjectsByType<UnitController>(FindObjectsSortMode.None);

            AP = teamController.availableAP;
        }

        public TurnDecision FindBestTurn(Vector2Int startPos, int maxMovement, float sprintBonus) {
            playerUnits = allUnits.Where(u => u.team == Team.Player && u.healthManager.GetHealth() > 0).ToList();
            Dictionary<Pathfinder.TileInfo, int> tileValues = new Dictionary<Pathfinder.TileInfo, int>();
            Dictionary<Pathfinder.TileInfo, TurnAction> bestActionPerTile = new Dictionary<Pathfinder.TileInfo, TurnAction>();

            findBestTile = new FindBestTile(personality, transform, playerUnits, attackManager);

            TurnDecision bestDecision = new();

            Dictionary<Vector2Int, Pathfinder.TileInfo> reachableTiles = pathfinder.ReturnReachableTiles(startPos, maxMovement, sprintBonus);

            // Add start tile to reachable tiles
            Vector2Int startTile = startPos;
            reachableTiles[startTile] = new Pathfinder.TileInfo {
                coords = startTile,
                reachable = true,
                sprintable = false
            };

            int remainingAP = AP;


            foreach (var kvp in reachableTiles) {
                int currentAP = AP;

                int tileValue = 0;

                Pathfinder.TileInfo info = kvp.Value;

                tileValue += findBestTile.EvaluateTileValue(info, playerUnits);
                currentAP -= MovementApUsed(info);

                TurnAction bestTurnAction = new TurnAction(TurnActionType.None);

                if (currentAP < 0) {
                    tileValue = 0; // Can't afford this tile, negative value
                }

                else if(currentAP - unitController.attackCost >= 0) {
                    findBestAction = new FindBestAction(unitController, personality, info, attackManager);
                    bestTurnAction = findBestAction.EvaluateBestActions(playerUnits);

                    currentAP = currentAP - unitController.attackCost;
                }

                tileValues[info] = tileValue + bestTurnAction.actionValue;
                bestActionPerTile[info] = bestTurnAction;
            }
            int maxValue = tileValues.Values.Max();

            var topTiles = tileValues.Where(kvp => kvp.Value == maxValue).Select(kvp => kvp.Key).ToList();

            foreach (var tile in topTiles) {
                Debug.Log($"Tile at {tile.coords} has value {tileValues[tile]} with action {bestActionPerTile[tile].type}");
            }

            if (topTiles.Count > 1) {
                //Debug.Log($"Multiple top tiles found with value {maxValue}. Choosing randomly among {topTiles.Count} options.");

                foreach (var tile in topTiles) {
                    if (tile.coords == GameContext.Instance.GridManager.WorldToGrid(this.transform.position)) { // one of the top tiles is the current position, prefer that one to avoid unnecessary movement
                        return CreateTurnDecision(tile, bestActionPerTile, tileValues, startPos);
                    }
                }

                System.Random random = new System.Random();

                int randomNumber = random.Next(0, topTiles.Count);

                Pathfinder.TileInfo chosenTile = topTiles[randomNumber];

                return CreateTurnDecision(chosenTile, bestActionPerTile, tileValues, startPos);
            }

            Pathfinder.TileInfo bestTile = tileValues.OrderByDescending(kvp => kvp.Value).First().Key;

            return CreateTurnDecision(bestTile, bestActionPerTile, tileValues, startPos);
        }


        // Determines the AP cost of moving to a tile based on whether it's reachable or sprintable.
        private int MovementApUsed(Pathfinder.TileInfo info) {
            if (info.reachable && !info.sprintable) {
                return 0;
            }
            else if (info.sprintable) {
                return unitController.sprintCost;
            }
            else return 0;
        }

        // sets info for turn decision
        private TurnDecision CreateTurnDecision(Pathfinder.TileInfo tile, Dictionary<Pathfinder.TileInfo, TurnAction> actions, Dictionary<Pathfinder.TileInfo, int> values, Vector2Int startPos, bool usePathfinder = true) {
            TurnDecision decision = new TurnDecision();
            decision.moveTile = tile;
            decision.routeToTile = usePathfinder ? pathfinder.FindPath(startPos, tile.coords) : new List<GridNode>();
            decision.action = actions[tile];
            decision.turnValue = values[tile];
            decision.movementState = tile.reachable == false ? UnitController.UnitState.Sprinting : UnitController.UnitState.Walking;
            return decision;
        }

    }
}   
