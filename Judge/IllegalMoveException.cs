using System;

namespace GameJudge
{
    [Serializable]
    public class IllegalMoveException : Exception 
    {
        public IllegalMoveException(string message) : base($"Illegal move: {message}")
        {

        }
    }
}
