using System;

namespace GameServer.GameLogic.Utils
{
    public class Randomizer
    {
        private static readonly Random rand = new Random();

        public static void Randomize<T>(T[] items)
        {
            
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }

        public static void RandomlyAssign<T>(T in1, T in2, out T out1, out T out2)
        {
            if (rand.Next(2) == 0)
            {
                out1 = in1;
                out2 = in2;
            }
            else
            {
                out1 = in2;
                out2 = in1;
            }
        }
    }
}
