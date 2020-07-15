using System.Collections.Generic;

namespace GameServer.GameLogic
{
    public class TroopSpawns
    {
        public static Dictionary<int, List<TroopTemplate>> TestPlanes(out int maxBlueWave, out int maxRedWave)
        {
            TroopTemplate blueTroop = new TroopTemplate(PlayerId.Blue, 5, 2, 0);
            TroopTemplate redTroop = new TroopTemplate(PlayerId.Red, 5, 2, 3);

            List<TroopTemplate> wave1 = new List<TroopTemplate>
            {
                blueTroop.Deploy(7, 7),
                redTroop.Deploy(11, 6),
                redTroop.Deploy(11, 7),
            };
            List<TroopTemplate> wave3 = new List<TroopTemplate>
            {
                blueTroop.Deploy(7, 7),
            };

            maxBlueWave = 3;
            maxRedWave = 1;

            return new Dictionary<int, List<TroopTemplate>>
            {
                {1, wave1 },
                {3, wave3 },
            };
        }
        
        public static Dictionary<int, List<TroopTemplate>> BasicPlanes(out int maxBlueWave, out int maxRedWave)
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

            maxBlueWave = 5;
            maxRedWave = 6;

            return new Dictionary<int, List<TroopTemplate>>
            {
                {1, wave1 },
                {3, wave3 },
                {4, wave4 },
                {5, wave5 },
                {6, wave6 },
            };
        }
    }
}