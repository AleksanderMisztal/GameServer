using System.Collections.Generic;
using GameJudge.Troops;
using GameJudge.Utils;

namespace GameJudge.WavesN
{
    public class Waves
    {
        private readonly Dictionary<int, List<Troop>> troopsForRound;

        public readonly int MaxRedWave;
        public readonly int MaxBlueWave;

        public Waves(Dictionary<int, List<Troop>> troopsForRound, int maxRedWave, int maxBlueWave)
        {
            this.troopsForRound = troopsForRound;
            MaxRedWave = maxRedWave;
            MaxBlueWave = maxBlueWave;
        }

        public List<Troop> GetTroops(int round)
        {
            try
            {
                return troopsForRound[round];
            }
            catch (KeyNotFoundException)
            {
                return new List<Troop>();
            }
        }


        public static Waves Test()
        {
            List<Troop> wave1 = new List<Troop>
            {
                TroopFactory.Blue(new VectorTwo(2, 3)),
                TroopFactory.Red(new VectorTwo(6, 2)),
                TroopFactory.Red(new VectorTwo(6, 3)),
            };
            List<Troop> wave3 = new List<Troop>
            {
                TroopFactory.Blue(new VectorTwo(2, 2)),
            };

            Dictionary<int, List<Troop>> troopsForRound = new Dictionary<int, List<Troop>>
            {
                {1, wave1 },
                {3, wave3 },
            };

            int maxRedWave = 1;
            int maxBlueWave = 3;

            return new Waves(troopsForRound, maxRedWave, maxBlueWave);
        }
        
        public static Waves Basic()
        {
            List<Troop> wave1 = new List<Troop>
            {
                TroopFactory.Blue(2, 5),
                TroopFactory.Blue(2, 6),
                TroopFactory.Blue(2, 7),
                TroopFactory.Blue(2, 8),
                TroopFactory.Red(16, 4),
                TroopFactory.Red(16, 5),
                TroopFactory.Red(16, 6),
                TroopFactory.Red(16, 7),
                TroopFactory.Red(16, 8),
            };
            List<Troop> wave3 = new List<Troop>
            {
                TroopFactory.Blue(2, 5),
                TroopFactory.Blue(2, 6),
                TroopFactory.Blue(2, 7),
                TroopFactory.Blue(2, 8),
            };
            List<Troop> wave4 = new List<Troop>
            {
                TroopFactory.Red(16, 4),
                TroopFactory.Red(16, 5),
                TroopFactory.Red(16, 6),
                TroopFactory.Red(16, 7),
            };
            List<Troop> wave5 = new List<Troop>
            {
                TroopFactory.Blue(2, 5),
                TroopFactory.Blue(2, 6),
                TroopFactory.Blue(2, 7),
                TroopFactory.Blue(2, 8),
            };
            List<Troop> wave6 = new List<Troop>
            {
                TroopFactory.Red(16, 5),
                TroopFactory.Red(16, 6),
                TroopFactory.Red(16, 7),
            };

            Dictionary<int, List<Troop>> troopsForRound = new Dictionary<int, List<Troop>>
            {
                {1, wave1 },
                {3, wave3 },
                {4, wave4 },
                {5, wave5 },
                {6, wave6 },
            };

            int maxRedWave = 6;
            int maxBlueWave = 5;

            return new Waves(troopsForRound, maxRedWave, maxBlueWave);
        }
    }
}