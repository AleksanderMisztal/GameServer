using GameServer.GameLogic.Troops;
using GameServer.GameLogic.Utils;

namespace GameServer.GameLogic.Battles
{
    public class AlwaysDamageBattles : IBattleResolver
    {
        public BattleResult GetFightResult(Troop defender, VectorTwo attackerPosition)
        {
            return new BattleResult(true, true);
        }

        public BattleResult GetCollisionResult()
        {
            return new BattleResult(true, true);
        }
    }
}
