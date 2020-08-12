namespace GameServer.GameLogic
{
    public interface IBattles
    {
        public BattleResult GetFightResult(Troop attacker, Troop defender);

        public BattleResult GetCollisionResult();
    }
}
