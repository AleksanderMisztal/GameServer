namespace GameServer.GameLogic
{
    public interface IBattleResolver
    {
        public BattleResult GetFightResult(Troop attacker, Troop defender);

        public BattleResult GetCollisionResult();
    }
}
