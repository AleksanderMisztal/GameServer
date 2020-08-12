using GameServer.Utils;
using System.Diagnostics;

namespace GameServer.GameLogic
{
    public class Troop
    {
        public PlayerId ControllingPlayer { get; }

        public int InitialMovePoints { get; private set; }
        public int MovePoints { get; private set; }

        public Vector2Int Position { get; private set; }
        public Vector2Int StartingPosition { get; set; }
        public int Orientation { get; private set; }

        public int Health { get; private set; }


        public Troop(TroopTemplate template)
        {
            ControllingPlayer = template.controllingPlayer;
            InitialMovePoints = template.movePoints;
            Health = template.health;
            Orientation = template.orientation;

            Position = template.position;
            StartingPosition = template.position;
        }

        public void JumpForward()
        {
            Position = Hex.GetAdjacentHex(Position, Orientation);
        }

        public void MoveInDirection(int direction)
        {
            if (MovePoints < 0)
            {
                throw new IllegalMoveException("Attempting to move a troop with no move points!");
            }
            if (MovePoints > 0) MovePoints--;

            Orientation = (6 + Orientation + direction) % 6;
            Position = Hex.GetAdjacentHex(Position, Orientation);
        }

        public Vector2Int GetAdjacentHex(int direction)
        {
            direction = (6 + Orientation + direction) % 6;
            return Hex.GetAdjacentHex(Position, direction);
        }

        public void ApplyDamage()
        {
            Health--;
            InitialMovePoints--;
            if (MovePoints > 0)
            {
                MovePoints--;
            }
        }

        public bool InControlZone(Vector2Int cell)
        {
            for (int rotation = -1; rotation <= 1; rotation++)
            {
                int dir = (6 + Orientation + rotation) % 6;
                Vector2Int controlledCell = Hex.GetAdjacentHex(Position, dir);
                if (cell == controlledCell) return true;
            }
            return false;
        }

        public void OnTurnEnd() { }

        public void OnTurnBegin()
        {
            MovePoints = InitialMovePoints;
        }


        public override string ToString()
        {
            return $"cp: {ControllingPlayer}, p: {Position}, o: {Orientation}, imp: {InitialMovePoints}, mp: {MovePoints}, h: {Health}";
        }
    }
}
