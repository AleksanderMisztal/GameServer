using GameServer.GameLogic;

namespace GameServer.Networking
{
    public class Game
    {
        public GameController Controller { get; }
        public int ClientRed { get; }
        public int ClientBlue { get; }

        public Game(GameController controller, int clientRed, int clientBlue)
        {
            Controller = controller;
            ClientRed = clientRed;
            ClientBlue = clientBlue;
        }
    }
}