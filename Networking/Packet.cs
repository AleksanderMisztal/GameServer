using System;
using System.Collections.Generic;
using System.Text;
using GameServer.GameLogic;
using GameServer.Utils;

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
        JoinLobby = 1,
        JoinGame = 2,
        MoveTroop = 3,
        SendMessage = 4,
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

        public Packet(int _id)
        {
            buffer = new List<byte>();
            readPos = 0;

            Write(_id);
        }

        public Packet(byte[] _data)
        {
            buffer = new List<byte>();
            readPos = 0;

            SetBytes(_data);
        }

        #region Functions
        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="_data">The bytes to add to the packet.</param>
        public void SetBytes(byte[] _data)
        {
            Write(_data);
            readableBuffer = buffer.ToArray();
        }

        /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the packet at the very beginning
        }

        /// <summary>Inserts the given int at the start of the buffer.</summary>
        /// <param name="_value">The int to insert.</param>
        public void InsertInt(int _value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(_value)); // Insert the int at the start of the buffer
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count; // Return the length of buffer
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - readPos; // Return the remaining length (unread)
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="_shouldReset">Whether or not to reset the packet.</param>
        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear(); // Clear buffer
                readableBuffer = null;
                readPos = 0; // Reset readPos
            }
            else
            {
                readPos -= 4; // "Unread" the last read int
            }
        }
        #endregion

        #region Write Data
        public void Write(byte _value)
        {
            buffer.Add(_value);
        }

        public void Write(byte[] _value)
        {
            buffer.AddRange(_value);
        }

        public void Write(short _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(long _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(string _value)
        {
            Write(_value.Length);
            buffer.AddRange(Encoding.ASCII.GetBytes(_value));
        }

        public void Write(Vector2Int _value)
        {
            Write(_value.X);
            Write(_value.Y);
        }

        public void Write(Troop _value)
        {
            Write((int)_value.Player);
            Write(_value.Health);
            Write(_value.InitialMovePoints);
            Write(_value.Orientation);
            Write(_value.Position);
        }

        public void Write(BattleResult _value)
        {
            Write(_value.AttackerDamaged);
            Write(_value.DefenderDamaged);
        }

        public void Write(Board _value)
        {
            Write(_value.xMin);
            Write(_value.xMax);
            Write(_value.yMin);
            Write(_value.yMax);
        }
        #endregion

        #region Read Data
        public byte ReadByte(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte _value = readableBuffer[readPos];
                if (_moveReadPos)
                {
                    readPos += 1;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] _value = buffer.GetRange(readPos, _length).ToArray();
                if (_moveReadPos)
                {
                    readPos += _length;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        public short ReadShort(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                short _value = BitConverter.ToInt16(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 2;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        public int ReadInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                int _value = BitConverter.ToInt32(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        public long ReadLong(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                long _value = BitConverter.ToInt64(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 8;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        public float ReadFloat(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float _value = BitConverter.ToSingle(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        public bool ReadBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 1;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt();
                string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length);
                if (_moveReadPos && _value.Length > 0)
                {
                    readPos += _length;
                }
                return _value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        public Vector2Int ReadVector2Int(bool _moveReadPos = true)
        {
            int x = ReadInt(_moveReadPos);
            int y = ReadInt(_moveReadPos);

            return new Vector2Int(x, y);
        }

        public BattleResult ReadBattleResult(bool _moveReadPos = true)
        {
            bool attackerDamaged = ReadBool(_moveReadPos);
            bool defenderDamaged = ReadBool(_moveReadPos);

            return new BattleResult(defenderDamaged, attackerDamaged);
        }
        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
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