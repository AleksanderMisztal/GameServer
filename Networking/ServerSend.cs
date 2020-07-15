using System.Collections.Generic;
using System.Threading.Tasks;
using GameServer.GameLogic;
using GameServer.Utils;

namespace GameServer.Networking
{
    class ServerSend
    {
        private static async Task SendDataWs(int toClient, Packet packet)
        {
            await Server.clients[toClient].wsClient.SendData(packet);
        }
        

        public static async Task Welcome(int toClient, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.Welcome))
            {
                packet.Write(message);
                packet.Write(toClient);

                await SendDataWs(toClient, packet);
            }
        }

        public static async Task GameJoined(int toClient, string oponentName, PlayerId side, BoardParams board)
        {
            using (Packet packet = new Packet((int)ServerPackets.GameJoined))
            {
                packet.Write(oponentName);
                packet.Write((int)side);
                packet.Write(board);

                await SendDataWs(toClient, packet);
            }
        }

        public static async Task TroopsSpawned(int toClient, List<TroopTemplate> templates)
        {
            using (Packet packet = new Packet((int)ServerPackets.TroopSpawned))
            {
                int length = templates.Count;
                packet.Write(length);

                for (int i = 0; i < length; i++)
                {
                    packet.Write(templates[i]);
                }

                await SendDataWs(toClient, packet);
            }
        }

        public static async Task TroopMoved(int toClient, Vector2Int position, int direction, List<BattleResult> battleResults)
        {
            using (Packet packet = new Packet((int)ServerPackets.TroopMoved))
            {
                packet.Write(position);
                packet.Write(direction);

                packet.Write(battleResults.Count);
                for (int i = 0; i < battleResults.Count; i++)
                {
                    packet.Write(battleResults[i]);
                }

                await SendDataWs(toClient, packet);
            }
        }

        public static async Task GameEnded(int toClient, int redScore, int blueScore)
        {
            using (Packet packet = new Packet((int)ServerPackets.GameEnded))
            {
                packet.Write(redScore);
                packet.Write(blueScore);

                await SendDataWs(toClient, packet);
            }
        }

        public static async Task MessageSent(int toClient, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.MessageSent))
            {
                packet.Write(message);

                await SendDataWs(toClient, packet);
            }
        }

        public static async Task OpponentDisconnected(int toClient)
        {
            using (Packet packet = new Packet((int)ServerPackets.OpponentDisconnected))
            {
                await SendDataWs(toClient, packet);
            }
        }
    }
}
