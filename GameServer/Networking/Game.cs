using System.Collections.Generic;
using System.Threading.Tasks;
using GameJudge;
using GameJudge.Areas;
using GameJudge.Utils;
using GameJudge.WavesN;

namespace GameServer.Networking
{
    public class Game
    {
        public readonly User RedUser;
        public readonly User BlueUser;
        private readonly Board board;
        private readonly Waves waves;

        private GameController controller;

        public Game(User redUser, User blueUser, Board board, Waves waves)
        {
            this.RedUser = redUser;
            this.BlueUser = blueUser;
            this.board = board;
            this.waves = waves;
        }

        public void MakeMove(int client, VectorTwo position, int direction)
        {
            PlayerSide player = GetColor(client);
            controller.ProcessMove(player, position, direction);
        }

        private PlayerSide GetColor(int client)
        {
            if (client == RedUser.id) return PlayerSide.Red;
            if (client == BlueUser.id) return PlayerSide.Blue;

            throw new KeyNotFoundException();
        }

        public async Task Initialize()
        {
            if (controller != null) return;

            controller = new GameController(waves, board);
            await ServerSend.GameJoined(RedUser.id, BlueUser.name, PlayerSide.Red, board);
            await ServerSend.GameJoined(BlueUser.id, RedUser.name, PlayerSide.Blue, board);

            controller.Initialize();
        }
    }
}