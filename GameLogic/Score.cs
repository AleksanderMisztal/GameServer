namespace GameServer.GameLogic
{
    public class Score
    {
        public int Red { get; private set; } = 0;
        public int Blue { get; private set; } = 0;

        public void Increment(PlayerSide player)
        {
            if (player == PlayerSide.Red) Red++;
            if (player == PlayerSide.Blue) Blue++;
        }
    }
}
