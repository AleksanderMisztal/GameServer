namespace GameJudge.Utils
{
    public class VectorTwo
    {
        public readonly int X;
        public readonly int Y;

        public int SqrMagnitude => X * X + Y * Y;


        public VectorTwo(int x, int y)
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
            if (obj == null || GetType() != obj.GetType()) return false;
            VectorTwo v = (VectorTwo)obj;
            return (X == v.X) && (Y == v.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static VectorTwo operator - (VectorTwo a, VectorTwo b)
            => new VectorTwo(a.X - b.X, a.Y - b.Y);


        public static bool operator == (VectorTwo a, VectorTwo b)
            => a.X == b.X && a.Y == b.Y;

        public static bool operator != (VectorTwo a, VectorTwo b)
            => a.X != b.X || a.Y != b.Y;
    }
}
