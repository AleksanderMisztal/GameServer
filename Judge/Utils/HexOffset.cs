using System;
using System.Linq;

namespace GameJudge.Utils
{
    public class HexOffset
    {
        private static readonly VectorTwo[] EvenSteps = {
            new VectorTwo(1, 0),
            new VectorTwo(0, 1),
            new VectorTwo(-1, 1),
            new VectorTwo(-1, 0),
            new VectorTwo(-1, -1),
            new VectorTwo(0, -1)
        };
        private static readonly VectorTwo[] OddSteps = {
            new VectorTwo(1, 0),
            new VectorTwo(1, 1),
            new VectorTwo(0, 1),
            new VectorTwo(-1, 0),
            new VectorTwo(0, -1),
            new VectorTwo(1, -1)
        };

        private readonly int x;
        private readonly int y;


        private HexOffset(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public HexOffset(VectorTwo v)
        {
            x = v.X;
            y = v.Y;
        }


        public HexOffset GetAdjacentHex(int direction)
        {
            direction %= 6;
            while (direction < 0) direction += 6;
            VectorTwo[] steps = (y & 1) == 1 ? OddSteps : EvenSteps;
            VectorTwo step = steps[direction % 6];
            return new HexOffset(x + step.X, y + step.Y);
        }

        public HexOffset[] GetNeighbors()
        {
            VectorTwo[] steps = (y & 1) == 1 ? OddSteps : EvenSteps;
            return steps.Select(s => new HexOffset(x + s.X, y + s.Y)).ToArray();
        }

        public VectorTwo ToVector()
        {
            return new VectorTwo(x, y);
        }

        public override string ToString()
        {
            return "Offset(" + x + ", " + y + ")";
        }
        
        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType())) return false;
            HexOffset offset = (HexOffset)obj;
            return (x == offset.x) && (y == offset.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }
    }
}
