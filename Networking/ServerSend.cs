using GameServer.GameLogic.ServerEvents;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace GameServer.Networking
{
    class ServerSend
    {
        public static async Task SendEvent(int toClient, IServerEvent ev)
        {
            using var packet = ev.GetPacket();
            try
            {
                await Server.clients[toClient].wsClient.SendData(packet);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("Exception thrown by server while sending data: " + ex);
                await GameHandler.ClientDisconnected(toClient);
            }
        }
    }
}
