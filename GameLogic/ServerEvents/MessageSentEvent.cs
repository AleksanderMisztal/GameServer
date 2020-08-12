using GameServer.Networking;
using System;

namespace GameServer.GameLogic.ServerEvents
{
    public class MessageSentEvent : IServerEvent
    {
        private string message;

        public MessageSentEvent(string message)
        {
            this.message = message;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.MessageSent);

            packet.Write(message);

            return packet;
        }

        public string GetString()
        {
            return "Messsage sent event";
        }
    }
}
