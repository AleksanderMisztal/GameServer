namespace GameServer.Networking.Packets
{
    public enum ServerPackets
    {
        Welcome = 1,
        GameJoined = 2,
        TroopSpawned = 3,
        TroopMoved = 4,
        GameEnded = 5,
        OpponentDisconnected = 6,
        MessageSent = 7,
        LostOnTime = 8,
    }
}