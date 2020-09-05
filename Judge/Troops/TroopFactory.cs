using GameJudge.Utils;

namespace GameJudge.Troops
{
    public static class TroopFactory
    {
        public static Troop Red(int x, int y)
        {
            return new Troop(PlayerSide.Red, 5, new VectorTwo(x, y), 3, 2);
        }

        public static Troop Blue(int x, int y)
        {
            return new Troop(PlayerSide.Blue, 5, new VectorTwo(x, y), 0, 2);
        }

        public static Troop Red(VectorTwo position)
        {
            return new Troop(PlayerSide.Red, 5, position, 3, 2);
        }

        public static Troop Blue(VectorTwo position)
        {
            return new Troop(PlayerSide.Blue, 5, position, 0, 2);
        }

        public static Troop Red(VectorTwo position, int orientation)
        {
            return new Troop(PlayerSide.Red, 5, position, orientation, 2);
        }

        public static Troop Blue(VectorTwo position, int orientation)
        {
            return new Troop(PlayerSide.Blue, 5, position, orientation, 2);
        }
    }
}