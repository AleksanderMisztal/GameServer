using GameServer.Utils;
using System.Diagnostics;

namespace GameServer.GameLogic
{
    public class MoveValidator
    {
        private readonly TroopMap map;
        private PlayerId activePlayer;

        public string Message { get; private set; } = null;


        public MoveValidator(TroopMap map, PlayerId player0)
        {
            this.map = map;
            this.activePlayer = player0;
        }

        public void ToggleActivePlayer()
        {
            activePlayer = activePlayer.Opponent();
        }

        public bool IsLegalMove(PlayerId player, Vector2Int position, int direction, Board board)
        {
            try
            {
                IsPlayersTurn(player);
                PositionContainsTroop(position);
                Troop troop = map.Get(position);
                PlayerControllsTroop(player, troop);
                TroopHasMovePoints(troop);
                NotBlockedByFriendsOrBoard(troop, direction, board);

                Message = "Move is valid.";
                return true;
            }
            catch (IllegalMoveException ex)
            {
                Message = ex.Message;
                Trace.WriteLine(Message);
                return false;
            }
        }


        private void IsPlayersTurn(PlayerId player)
        {
            if (player != activePlayer)
                throw new IllegalMoveException("Attempting to make a move in oponent's turn!");
        }

        private void PositionContainsTroop(Vector2Int position)
        {
            if (map.Get(position) == null)
                throw new IllegalMoveException("No troop at the specified hex!");
        }

        private void PlayerControllsTroop(PlayerId player, Troop troop)
        {
            if (player != troop.Player)
                throw new IllegalMoveException("Attempting to move enemy troop!");
        }

        private void TroopHasMovePoints(Troop troop)
        {
            if (troop.MovePoints <= 0)
                throw new IllegalMoveException("Attempting to move a troop with no move points!");
        }

        // TODO: fix logic 
        private void NotBlockedByFriendsOrBoard(Troop troop, int direction, Board board)
        {
            Vector2Int targetPosition = troop.GetAdjacentHex(direction);
            Troop encounter = map.Get(targetPosition);
            if (encounter != null && encounter.Player == troop.Player)
            {
                foreach (var cell in Hex.GetControllZone(troop.Position, troop.Orientation))
                {
                    encounter = map.Get(targetPosition);
                    if (!targetPosition.IsOutside(board)
                        && (encounter != null || encounter.Player != troop.Player))
                    {
                        throw new IllegalMoveException("Attempting to enter a cell with friendly troop!");
                    }
                }
            }
        }
    }
}
