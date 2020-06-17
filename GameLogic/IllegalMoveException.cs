using System;

namespace GameServer.GameLogic
{
    [Serializable]
    public class IllegalMoveException : Exception 
    {
        public IllegalMoveException(string message) : base($"Illegal move: {message}")
        {

        }
    }
}
