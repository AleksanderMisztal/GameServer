using GameServer.Utils;

namespace GameServer.GameLogic
{
    public class TroopAi
    {
        private readonly TroopMap troopMap;
        private readonly Board board;

        public TroopAi(TroopMap troopMap, Board board)
        {
            this.troopMap = troopMap;
            this.board = board;
        }

        public bool ShouldControllWithAi(Troop troop)
        {
            if (board.IsOutside(troop.Position)) return true;

            foreach (var cell in Hex.GetControllZone(troop.Position, troop.Orientation))
            {
                if (!board.IsOutside(cell))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
