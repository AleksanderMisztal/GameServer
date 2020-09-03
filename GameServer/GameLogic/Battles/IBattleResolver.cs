using GameServer.GameLogic.Troops;
using GameServer.GameLogic.Utils;

namespace GameServer.GameLogic.Battles
{
    public interface IBattleResolver
    {
        public BattleResult GetFightResult(Troop defender, VectorTwo attackerPosition);

        public BattleResult GetCollisionResult();
    }
}
