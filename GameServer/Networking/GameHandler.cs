using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic;
using System.Threading.Tasks;
using GameServer.GameLogic.ServerEvents;
using System;

namespace GameServer.Networking
{
    public static class GameHandler
    {
        private static readonly Dictionary<int, Game> clientToGame = new Dictionary<int, Game>();

        private static bool someoneWaiting = false;
        private static User waitingUser;

        public static async Task SendToGame(User newUser)
        {
            if (someoneWaiting)
            {
                someoneWaiting = false;
                Randomizer.RandomlyAssign(newUser, waitingUser, out User redUser, out User blueUser);
                await InitializeNewGame(redUser, blueUser);
            }
            else
            {
                someoneWaiting = true;
                waitingUser = newUser;
            }
        }

        private static async Task InitializeNewGame(User playingRed, User playingBlue)
        {
            Waves waves = Waves.Basic();
            Board board = Board.standard;
            Game game = new Game(playingRed, playingBlue, board, waves);

            clientToGame[playingRed.id] = game;
            clientToGame[playingBlue.id] = game;

            await game.Initialize();
        }

        public static async Task MoveTroop(int client, Vector2Int position, int direction)
        {
            if (direction < -1 || direction > 1)
            {
                Console.WriteLine($"Client {client} sent a move with illegal direction!");
                return;
            }
            Game game = clientToGame[client];
            List<IGameEvent> events = game.MakeMove(client, position, direction);
            foreach (var ev in events)
            {
                await ServerSend.GameEvent(game.blueUser.id, ev);
                await ServerSend.GameEvent(game.redUser.id, ev);
            }
        }

        public static async Task SendMessage(int client, string message)
        {
            int oponent = GetOpponent(client);
            await ServerSend.MessageSent(oponent, message);
        }

        public static async Task ClientDisconnected(int client)
        {
            if (someoneWaiting && client == waitingUser.id)
                someoneWaiting = false;

            int oponent = GetOpponent(client);

            clientToGame.Remove(client);
            clientToGame.Remove(oponent);

            await ServerSend.OpponentDisconnected(oponent);
        }

        private static int GetOpponent(int client)
        {
            Game game = clientToGame[client];
            int oponentId = game.blueUser.id ^ game.redUser.id ^ client;
            return oponentId;
        }
    }
}