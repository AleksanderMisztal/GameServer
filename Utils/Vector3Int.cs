namespace GameServer.Utils
{
    class Vector3Int
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3Int()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Vector3Int Abs()
        {
            return new Vector3Int(
                X > 0 ? X : -X,
                Y > 0 ? Y : -Y,
                Z > 0 ? Z : -Z
            );
        }

        public static Vector3Int operator +(Vector3Int a, Vector3Int b)
            => new Vector3Int(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3Int operator -(Vector3Int a, Vector3Int b)
            => new Vector3Int(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
}
