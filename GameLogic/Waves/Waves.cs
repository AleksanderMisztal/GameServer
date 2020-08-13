using GameServer.Utils;
using System.Collections.Generic;

namespace GameServer.GameLogic
{
    public class Waves
    {
        public readonly Dictionary<int, List<Troop>> troopsForRound;

        public readonly int maxRedWave;
        public readonly int maxBlueWave;

        public Waves(Dictionary<int, List<Troop>> troopsForRound, int maxRedWave, int maxBlueWave)
        {
            this.troopsForRound = troopsForRound;
            this.maxRedWave = maxRedWave;
            this.maxBlueWave = maxBlueWave;
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
                Troop.Blue(new Vector2Int(2, 3)),
                Troop.Red(new Vector2Int(6, 3)),
                Troop.Blue(new Vector2Int(6, 2)),
            };
            List<Troop> wave3 = new List<Troop>
            {
                Troop.Blue(new Vector2Int(2, 2)),
            };

            var troopsForRound = new Dictionary<int, List<Troop>>
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
                Troop.Blue(2, 5),
                Troop.Blue(2, 6),
                Troop.Blue(2, 7),
                Troop.Blue(2, 8),
                Troop.Red(16, 4),
                Troop.Red(16, 5),
                Troop.Red(16, 6),
                Troop.Red(16, 7),
                Troop.Red(16, 8),
            };
            List<Troop> wave3 = new List<Troop>
            {
                Troop.Blue(2, 5),
                Troop.Blue(2, 6),
                Troop.Blue(2, 7),
                Troop.Blue(2, 8),
            };
            List<Troop> wave4 = new List<Troop>
            {
                Troop.Red(16, 4),
                Troop.Red(16, 5),
                Troop.Red(16, 6),
                Troop.Red(16, 7),
            };
            List<Troop> wave5 = new List<Troop>
            {
                Troop.Blue(2, 5),
                Troop.Blue(2, 6),
                Troop.Blue(2, 7),
                Troop.Blue(2, 8),
            };
            List<Troop> wave6 = new List<Troop>
            {
                Troop.Red(16, 5),
                Troop.Red(16, 6),
                Troop.Red(16, 7),
            };

            var troopsForRound = new Dictionary<int, List<Troop>>
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