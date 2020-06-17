using GameServer.Utils;
using System.Collections.Generic;

namespace GameServer.GameLogic
{
    public class Wave
    {
        public IEnumerable<TroopTemplate> troopTemplates;
        public Vector2Int spawnPosition;

        public Wave(IEnumerable<TroopTemplate> troops, Vector2Int spawnPosition)
        {
            this.troopTemplates = troops;
            this.spawnPosition = spawnPosition;
        }


        public static Dictionary<int, Wave> BasicPlanes(out int maxBlueWave, out int maxRedWave)
        {
            TroopTemplate blueTroop = new TroopTemplate(PlayerId.Blue, 5, 2, 0);
            TroopTemplate redTroop = new TroopTemplate(PlayerId.Red, 4, 2, 3);

            List<TroopTemplate> blueWaveTroops = new List<TroopTemplate>
            {
                blueTroop, blueTroop, blueTroop, blueTroop
            };
            List<TroopTemplate> redWave1Troops = new List<TroopTemplate>
            {
                redTroop, redTroop, redTroop, redTroop, redTroop
            };
            List<TroopTemplate> redWave2Troops = new List<TroopTemplate>
            {
                redTroop, redTroop, redTroop, redTroop
            };
            List<TroopTemplate> redWave3Troops = new List<TroopTemplate>
            {
                redTroop, redTroop, redTroop
            };

            Wave blueWave = new Wave(blueWaveTroops, new Vector2Int(8, 7));
            Wave redWave1 = new Wave(redWave1Troops, new Vector2Int(18, 7));
            Wave redWave2 = new Wave(redWave2Troops, new Vector2Int(18, 10));
            Wave redWave3 = new Wave(redWave3Troops, new Vector2Int(18, 4));

            maxBlueWave = 5;
            maxRedWave = 6;
            return new Dictionary<int, Wave>
            {
                {1, blueWave },
                {2, redWave1 },
                {3, blueWave },
                {4, redWave2 },
                {5, blueWave },
                {6, redWave3 },
            };
        }

        public static Dictionary<int, Wave> TestPlanes(out int maxBlueWave, out int maxRedWave)
        {
            TroopTemplate blueTroop = new TroopTemplate(PlayerId.Blue, 2, 2, 0);
            TroopTemplate redTroop = new TroopTemplate(PlayerId.Red, 2, 2, 3);

            List<TroopTemplate> blueWaveTroops = new List<TroopTemplate>
            {
                blueTroop, blueTroop
            };
            List<TroopTemplate> redWave1Troops = new List<TroopTemplate>
            {
                redTroop
            };
            List<TroopTemplate> redWave2Troops = new List<TroopTemplate>
            {
                redTroop, redTroop
            };
            List<TroopTemplate> redWave3Troops = new List<TroopTemplate>
            {
                redTroop
            };

            Wave blueWave = new Wave(blueWaveTroops, new Vector2Int(10, 5));
            Wave redWave1 = new Wave(redWave1Troops, new Vector2Int(12, 5));
            Wave redWave2 = new Wave(redWave2Troops, new Vector2Int(12, 5));
            Wave redWave3 = new Wave(redWave3Troops, new Vector2Int(12, 5));

            maxBlueWave = 1;
            maxRedWave = 2;
            return new Dictionary<int, Wave>
            {
                {1, blueWave },
                {2, redWave1 },
            };
        }
    }
}