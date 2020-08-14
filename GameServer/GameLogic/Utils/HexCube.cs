namespace GameServer.Utils
{
    class HexCube
    {
        private static readonly Vector3Int[] steps = {
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(0, -1, 1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, 1)
        };

        private readonly int q;
        private readonly int r;
        private readonly int s;


        public HexCube(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
        }

        public HexCube(Vector3Int v)
        {
            this.q = v.X;
            this.r = v.Y;
            this.s = v.Z;
        }


        public Vector3Int ToVector()
        {
            return new Vector3Int(q, r, s);
        }

        public HexOffset ToOffset()
        {
            int y = r;
            int x = q + (y - (y & 1)) / 2;
            return new HexOffset(x, y);
        }

        public override string ToString()
        {
            return "HexCube(" + q + ", " + r + ", " + s + ")";
        }
    }
}
