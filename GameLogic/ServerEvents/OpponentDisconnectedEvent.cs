using GameServer.Networking;
using System;

namespace GameServer.GameLogic.ServerEvents
{
    public class OpponentDisconnectedEvent : IServerEvent
    {
        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.OpponentDisconnected);

            return packet;
        }

        public string GetString()
        {
            return "Opponent disconnected event";
        }
    }
}
