using System.Collections.Generic;
using System.Threading.Tasks;
using GameServer.GameLogic;
using GameServer.GameLogic.GameEvents;
using GameServer.GameLogic.Utils;
using GameServer.GameLogic.Waves;

namespace GameServer.Networking
{
    public class Game
    {
        public readonly User redUser;
        public readonly User blueUser;
        private readonly Board board;
        private readonly Waves waves;

        private GameController controller = null;

        public Game(User redUser, User blueUser, Board board, Waves waves)
        {
            this.redUser = redUser;
            this.blueUser = blueUser;
            this.board = board;
            this.waves = waves;
        }

        public List<IGameEvent> MakeMove(int client, Vector2Int position, int direction)
        {
            PlayerSide player = GetColor(client);
            var events = controller.ProcessMove(player, position, direction);
            return events;
        }

        private PlayerSide GetColor(int client)
        {
            if (client == redUser.id) return PlayerSide.Red;
            if (client == blueUser.id) return PlayerSide.Blue;

            throw new KeyNotFoundException();
        }

        public async Task Initialize()
        {
            if (controller != null) return;

            controller = new GameController(waves, board);
            await ServerSend.GameJoined(redUser.id, blueUser.name, PlayerSide.Red, board);
            await ServerSend.GameJoined(blueUser.id, redUser.name, PlayerSide.Blue, board);

            var ev = controller.InitializeAndReturnEvent();
            await ServerSend.GameEvent(redUser.id, ev);
            await ServerSend.GameEvent(blueUser.id, ev);
        }
    }
}