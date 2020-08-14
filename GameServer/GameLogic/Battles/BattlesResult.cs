namespace GameServer.GameLogic
{
    public class BattleResult
    {
        public bool DefenderDamaged { get; set; }
        public bool AttackerDamaged { get; set; }
        public static BattleResult FriendlyCollision => new BattleResult(true, true);

        public BattleResult(bool defenderDamaged, bool attackerDamaged)
        {
            DefenderDamaged = defenderDamaged;
            AttackerDamaged = attackerDamaged;
        }

        public BattleResult() 
        {
            DefenderDamaged = false;
            AttackerDamaged = false;
        }

        public override string ToString()
        {
            return $"a: {AttackerDamaged} b: {DefenderDamaged}";
        }
    }
}
