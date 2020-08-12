using System.Collections.Generic;

namespace GameServer.GameLogic
{
    public class Waves
    {
        public readonly Dictionary<int, List<TroopTemplate>> troopsForRound;

        public readonly int maxRedWave;
        public readonly int maxBlueWave;

        public Waves(Dictionary<int, List<TroopTemplate>> troopsForRound, int maxRedWave, int maxBlueWave)
        {
            this.troopsForRound = troopsForRound;
            this.maxRedWave = maxRedWave;
            this.maxBlueWave = maxBlueWave;
        }

        public static Waves Test()
        {
            TroopTemplate blueTroop = new TroopTemplate(PlayerId.Blue, 5, 2, 0);
            TroopTemplate redTroop = new TroopTemplate(PlayerId.Red, 5, 2, 3);

            List<TroopTemplate> wave1 = new List<TroopTemplate>
            {
                blueTroop.Deploy(2, 3),
                redTroop.Deploy(6, 3),
                redTroop.Deploy(6, 2),
            };
            List<TroopTemplate> wave3 = new List<TroopTemplate>
            {
                blueTroop.Deploy(2, 2),
            };

            var troopsForRound = new Dictionary<int, List<TroopTemplate>>
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
            TroopTemplate blueTroop = new TroopTemplate(PlayerId.Blue, 5, 2, 0);
            TroopTemplate redTroop = new TroopTemplate(PlayerId.Red, 5, 2, 3);

            List<TroopTemplate> wave1 = new List<TroopTemplate>
            {
                blueTroop.Deploy(2, 5),
                blueTroop.Deploy(2, 6),
                blueTroop.Deploy(2, 7),
                blueTroop.Deploy(2, 8),
                redTroop.Deploy(16, 4),
                redTroop.Deploy(16, 5),
                redTroop.Deploy(16, 6),
                redTroop.Deploy(16, 7),
                redTroop.Deploy(16, 8)
            };
            List<TroopTemplate> wave3 = new List<TroopTemplate>
            {
                blueTroop.Deploy(2, 5),
                blueTroop.Deploy(2, 6),
                blueTroop.Deploy(2, 7),
                blueTroop.Deploy(2, 8)
            };
            List<TroopTemplate> wave4 = new List<TroopTemplate>
            {
                redTroop.Deploy(16, 4),
                redTroop.Deploy(16, 5),
                redTroop.Deploy(16, 6),
                redTroop.Deploy(16, 7)
            };
            List<TroopTemplate> wave5 = new List<TroopTemplate>
            {
                blueTroop.Deploy(2, 5),
                blueTroop.Deploy(2, 6),
                blueTroop.Deploy(2, 7),
                blueTroop.Deploy(2, 8)
            };
            List<TroopTemplate> wave6 = new List<TroopTemplate>
            {
                redTroop.Deploy(16, 5),
                redTroop.Deploy(16, 6),
                redTroop.Deploy(16, 7)
            };

            var troopsForRound = new Dictionary<int, List<TroopTemplate>>
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