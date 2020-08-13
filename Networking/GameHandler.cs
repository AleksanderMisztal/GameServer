using System;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic;
using System.Threading.Tasks;
using GameServer.GameLogic.ServerEvents;

namespace GameServer.Networking
{
    public static class GameHandler
    {
        private static readonly Dictionary<int, Game> clientToGame = new Dictionary<int, Game>();
        private static readonly Dictionary<int, PlayerSide> clientToColor = new Dictionary<int, PlayerSide>();
        private static readonly Dictionary<int, string> clientToUsername = new Dictionary<int, string>();

        private static bool someoneWaiting = false;
        private static int waitingClient;


        public static void AddToLobby(int client, string username)
        {
            clientToUsername[client] = username;
        }

        public static async Task SendToGame(int client)
        {
            if (!someoneWaiting)
            {
                someoneWaiting = true;
                waitingClient = client;
                return;
            }

            someoneWaiting = false;

            int playingRed;
            int playingBlue;

            if (new Random().Next(2) == 0)
            {
                playingRed = waitingClient;
                playingBlue = client;
            }
            else
            {
                playingRed = client;
                playingBlue = waitingClient;
            }

            Waves waves = Waves.Basic();
            Board board = Board.standard;

            Game game = new Game(new GameController(waves, board), playingRed, playingBlue);


            clientToGame[playingRed] = game;
            clientToGame[playingBlue] = game;

            clientToColor[playingBlue] = PlayerSide.Blue;
            clientToColor[playingRed] = PlayerSide.Red;


            var redGameJoined = new GameJoinedEvent(clientToUsername[playingBlue], PlayerSide.Red, board);
            var blueGameJoined = new GameJoinedEvent(clientToUsername[playingRed], PlayerSide.Blue, board);

            await Server.SendEvent(playingRed, redGameJoined);
            await Server.SendEvent(playingBlue, blueGameJoined);

            var ev = game.Controller.InitializeAndReturnEvent();

            await Server.SendEvent(playingRed, ev);
            await Server.SendEvent(playingBlue, ev);
        }

        public static async Task MoveTroop(int client, Vector2Int position, int direction)
        {
            if (clientToGame.TryGetValue(client, out Game game))
            {
                PlayerSide color = clientToColor[client];
                List<IServerEvent> events = game.Controller.ProcessMove(color, position, direction);
                foreach (var ev in events)
                {
                    await Server.SendEvent(game.ClientBlue, ev);
                    await Server.SendEvent(game.ClientRed, ev);
                }
            }
        }

        public static async Task SendMessage(int client, string message)
        {
            if (clientToGame.TryGetValue(client, out Game game))
            {
                int oponent = game.ClientBlue ^ game.ClientRed ^ client;
                await Server.SendEvent(oponent, new MessageSentEvent(message));
            }
            else
            {
                await Server.SendEvent(client, new OpponentDisconnectedEvent());
            }
        }

        public static async Task ClientDisconnected(int clientId)
        {
            if (someoneWaiting && clientId == waitingClient)
                someoneWaiting = false;

            if (clientToGame.TryGetValue(clientId, out Game game))
            {
                int oponent = clientId ^ game.ClientBlue ^ game.ClientRed;

                clientToGame.Remove(clientId);
                clientToGame.Remove(oponent);

                await Server.SendEvent(oponent, new OpponentDisconnectedEvent());
            }
        }
    }
}