using System.Collections.Generic;
using GameJudge;
using GameJudge.Utils;

namespace GameServer.Networking
{
    public class Game
    {
        public readonly User RedUser;
        public readonly User BlueUser;
        private readonly GameController controller;

        public Game(User redUser, User blueUser, GameController controller)
        {
            RedUser = redUser;
            BlueUser = blueUser;
            this.controller = controller;
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
    }
}