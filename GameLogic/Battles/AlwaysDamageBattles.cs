namespace GameServer.GameLogic.Battles
{
    public class AlwaysDamageBattles : IBattles
    {
        public BattleResult GetCollisionResult()
        {
            return new BattleResult(true, true);
        }

        public BattleResult GetFightResult(Troop attacker, Troop defender)
        {
            return new BattleResult(true, true);
        }
    }
}
