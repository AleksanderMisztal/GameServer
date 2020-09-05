using System;
using System.Collections.Generic;
using System.Text;
using GameJudge.Troops;

namespace GameJudge.GameEvents
{
    public class TroopsSpawnedEventArgs : EventArgs
    {
        public readonly List<Troop> Troops;

        public TroopsSpawnedEventArgs(List<Troop> troops)
        {
            Troops = troops;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("New round event\n");
            foreach (Troop t in Troops) sb.Append(t).Append("\n");
            return sb.ToString();
        }
    }
}
