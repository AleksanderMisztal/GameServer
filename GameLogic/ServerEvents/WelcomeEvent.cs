using GameServer.Networking;
using System;

namespace GameServer.GameLogic.ServerEvents
{
    public class WelcomeEvent : IServerEvent
    {
        private string message;

        public WelcomeEvent(string message)
        {
            this.message = message;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.Welcome);

            packet.Write(message);

            return packet;
        }

        public string GetString()
        {
            return "Welcome event";
        }
    }
}
