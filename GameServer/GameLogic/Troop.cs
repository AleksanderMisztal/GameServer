using GameServer.Utils;

namespace GameServer.GameLogic
{
    public class Troop
    {
        public PlayerSide Player { get; }

        public int InitialMovePoints { get; private set; }
        public int MovePoints { get; private set; }

        public Vector2Int Position { get; set; }
        public Vector2Int StartingPosition { get; set; }
        public int Orientation { get; private set; }

        public int Health { get; private set; }


        public Troop(PlayerSide player, int movePoints, Vector2Int position, int orientation, int health)
        {
            Player = player;
            InitialMovePoints = movePoints;
            MovePoints = movePoints;
            Position = position;
            StartingPosition = position;
            Orientation = orientation;
            Health = health;
        }

        public void MoveInDirection(int direction)
        {
            if (MovePoints <= 0)
                throw new IllegalMoveException("I have no move points!");

            MovePoints--;
            Orientation = (6 + Orientation + direction) % 6;
            Position = Hex.GetAdjacentHex(Position, Orientation);
        }

        public void FlyOverOtherTroop()
        {
            Position = Hex.GetAdjacentHex(Position, Orientation);
        }

        public void ApplyDamage()
        {
            Health--;
            InitialMovePoints--;
            if (MovePoints > 0)
                MovePoints--;
        }

        public Vector2Int[] ControllZone => Hex.GetControllZone(Position, Orientation);

        public bool InControlZone(Vector2Int position)
        {
            foreach (var cell in ControllZone)
                if (cell == position)
                    return true;

            return false;
        }

        public void ResetMovePoints()
        {
            MovePoints = InitialMovePoints;
        }


        // Factories
        public static Troop Red(int x, int y)
        {
            return new Troop(PlayerSide.Red, 5, new Vector2Int(x, y), 3, 2);
        }

        public static Troop Blue(int x, int y)
        {
            return new Troop(PlayerSide.Blue, 5, new Vector2Int(x, y), 0, 2);
        }

        public static Troop Red(Vector2Int position)
        {
            return new Troop(PlayerSide.Red, 5, position, 3, 2);
        }

        public static Troop Blue(Vector2Int position)
        {
            return new Troop(PlayerSide.Blue, 5, position, 0, 2);
        }

        public static Troop Red(Vector2Int position, int orientation)
        {
            return new Troop(PlayerSide.Red, 5, position, orientation, 2);
        }

        public static Troop Blue(Vector2Int position, int orientation)
        {
            return new Troop(PlayerSide.Blue, 5, position, orientation, 2);
        }

    }
}
