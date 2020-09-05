using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using GameJudge.Areas;
using GameJudge.Utils;
using GameJudge.WavesN;

namespace GameServer.Networking
{
    public static class GameHandler
    {
        private static readonly Dictionary<int, Game> ClientToGame = new Dictionary<int, Game>();

        private static bool _someoneWaiting;
        private static User _waitingUser;

        public static async Task SendToGame(User newUser)
        {
            if (_someoneWaiting)
            {
                _someoneWaiting = false;
                Randomizer.RandomlyAssign(newUser, _waitingUser, out User redUser, out User blueUser);
                await InitializeNewGame(redUser, blueUser);
            }
            else
            {
                _someoneWaiting = true;
                _waitingUser = newUser;
            }
        }

        private static async Task InitializeNewGame(User playingRed, User playingBlue)
        {
            Waves waves = Waves.Test();
            Board board = Board.Test;
            Game game = new Game(playingRed, playingBlue, board, waves);

            ClientToGame[playingRed.id] = game;
            ClientToGame[playingBlue.id] = game;

            await game.Initialize();
        }

        public static void MoveTroop(int client, VectorTwo position, int direction)
        {
            if (direction < -1 || direction > 1)
            {
                Console.WriteLine($"Client {client} sent a move with illegal direction!");
                return;
            }
            Game game = ClientToGame[client];
            game.MakeMove(client, position, direction);
        }

        public static async Task SendMessage(int client, string message)
        {
            int opponent = GetOpponent(client);
            await ServerSend.MessageSent(opponent, message);
        }

        public static async Task ClientDisconnected(int client)
        {
            if (_someoneWaiting && client == _waitingUser.id)
                _someoneWaiting = false;

            int opponent = GetOpponent(client);

            ClientToGame.Remove(client);
            ClientToGame.Remove(opponent);

            await ServerSend.OpponentDisconnected(opponent);
        }

        private static int GetOpponent(int client)
        {
            Game game = ClientToGame[client];
            int opponentId = game.BlueUser.id ^ game.RedUser.id ^ client;
            return opponentId;
        }
    }
}