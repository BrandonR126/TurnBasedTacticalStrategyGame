namespace AI.Data {
    public enum TurnActionType {
        None,
        Attack
    }

    public class TurnAction {
        public TurnActionType type;
        public UnitController target;

        public int actionValue;
        public int damage;
        public int cost;

        public TurnAction(TurnActionType type, UnitController target = null, int actionValue = 0 , int damage = 0, int cost = 0) {
            this.type = type;
            this.target = target;
            this.actionValue = actionValue;
            this.damage = damage;
            this.cost = cost;
        }
    }
}
