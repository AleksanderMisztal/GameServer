namespace GameServer.GameLogic.Utils
{
    public class Vector2Int
    {
        public int X { get; }
        public int Y { get; }

        public int SqrMagnitude => X * X + Y * Y;


        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return 1000 * (X + 100) + Y + 100;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType())) return false;
            Vector2Int v = (Vector2Int)obj;
            return (X == v.X) && (Y == v.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }


        public static Vector2Int operator + (Vector2Int a, Vector2Int b)
            => new Vector2Int(a.X + b.X, a.Y + b.Y);

        public static Vector2Int operator - (Vector2Int a, Vector2Int b)
            => new Vector2Int(a.X - b.X, a.Y - b.Y);


        public static bool operator == (Vector2Int a, Vector2Int b)
            => a.X == b.X && a.Y == b.Y;

        public static bool operator != (Vector2Int a, Vector2Int b)
            => a.X != b.X || a.Y != b.Y;
    }
}
