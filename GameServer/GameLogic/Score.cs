namespace GameServer.GameLogic
{
    public class Score
    {
        public int Red { get; private set; }
        public int Blue { get; private set; }

        public void Increment(PlayerSide player)
        {
            if (player == PlayerSide.Red) Red++;
            if (player == PlayerSide.Blue) Blue++;
        }
    }
}
