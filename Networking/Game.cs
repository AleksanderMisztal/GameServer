using System.Collections.Generic;
using System.Threading.Tasks;
using GameServer.GameLogic;
using GameServer.GameLogic.ServerEvents;
using GameServer.Utils;

namespace GameServer.Networking
{
    public class Game
    {
        private readonly GameController controller;
        public readonly User redUser;
        public readonly User blueUser;

        public Game(GameController controller, User redUser, User blueUser)
        {
            this.controller = controller;
            this.redUser = redUser;
            this.blueUser = blueUser;
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

        public async Task Initialize(Board board)
        {
            await ServerSend.GameJoined(redUser.id, blueUser.name, PlayerSide.Red, board);
            await ServerSend.GameJoined(blueUser.id, redUser.name, PlayerSide.Blue, board);

            var ev = controller.InitializeAndReturnEvent();
            await ServerSend.GameEvent(redUser.id, ev);
            await ServerSend.GameEvent(blueUser.id, ev);
        }
    }
}