using System;
using System.Collections.Generic;
using System.Text;
using GameServer.GameLogic;
using GameServer.GameLogic.Battles;
using GameServer.GameLogic.Utils;

namespace GameServer.Networking
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

    public enum ClientPackets
    {
        JoinGame = 1,
        MoveTroop = 2,
        SendMessage = 3,
    }

    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        public Packet()
        {
            buffer = new List<byte>();
            readPos = 0;
        }

        public Packet(int id)
        {
            buffer = new List<byte>();
            readPos = 0;

            Write(id);
        }

        public Packet(byte[] data)
        {
            buffer = new List<byte>();
            readPos = 0;

            SetBytes(data);
        }

        #region Functions
        public void SetBytes(byte[] data)
        {
            Write(data);
            readableBuffer = buffer.ToArray();
        }

        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
        }

        public void InsertInt(int value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(value));
        }

        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        public int Length()
        {
            return buffer.Count;
        }

        public int UnreadLength()
        {
            return Length() - readPos;
        }

        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                buffer.Clear();
                readableBuffer = null;
                readPos = 0;
            }
            else
            {
                readPos -= 4;
            }
        }
        #endregion

        #region Write Data
        public void Write(byte value)
        {
            buffer.Add(value);
        }

        public void Write(byte[] value)
        {
            buffer.AddRange(value);
        }

        public void Write(short value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            Write(value.Length);
            buffer.AddRange(Encoding.ASCII.GetBytes(value));
        }

        public void Write(Vector2Int value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public void Write(Troop value)
        {
            Write((int)value.Player);
            Write(value.Health);
            Write(value.InitialMovePoints);
            Write(value.Orientation);
            Write(value.Position);
        }

        public void Write(BattleResult value)
        {
            Write(value.AttackerDamaged);
            Write(value.DefenderDamaged);
        }

        public void Write(Board value)
        {
            Write(value.xMax);
            Write(value.yMax);
        }
        #endregion

        #region Read Data
        public byte ReadByte(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte value = readableBuffer[readPos];
                if (moveReadPos)
                {
                    readPos += 1;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] value = buffer.GetRange(readPos, length).ToArray();
                if (moveReadPos)
                {
                    readPos += length;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        public short ReadShort(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                short value = BitConverter.ToInt16(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 2;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        public int ReadInt(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                int value = BitConverter.ToInt32(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        public long ReadLong(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                long value = BitConverter.ToInt64(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 8;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        public float ReadFloat(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float value = BitConverter.ToSingle(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        public bool ReadBool(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool value = BitConverter.ToBoolean(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 1;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                int length = ReadInt();
                string value = Encoding.ASCII.GetString(readableBuffer, readPos, length);
                if (moveReadPos && value.Length > 0)
                {
                    readPos += length;
                }
                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        public Vector2Int ReadVector2Int()
        {
            int x = ReadInt();
            int y = ReadInt();

            return new Vector2Int(x, y);
        }

        public Troop ReadTroop()
        {
            PlayerSide side = (PlayerSide)ReadInt();
            int health = ReadInt();
            int initialMovePoints = ReadInt();
            int orientation = ReadInt();
            Vector2Int position = ReadVector2Int();

            return new Troop(side, initialMovePoints, position, orientation, health);
        }

        public BattleResult ReadBattleResult()
        {
            bool attackerDamaged = ReadBool();
            bool defenderDamaged = ReadBool();

            return new BattleResult(defenderDamaged, attackerDamaged);
        }

        public Board ReadBoard()
        {
            int xMax = ReadInt();
            int yMax = ReadInt();

            return new Board(xMax, yMax);
        }
        #endregion

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}