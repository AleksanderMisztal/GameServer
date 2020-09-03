using System;
using System.Collections.Generic;
using System.Text;
using GameServer.GameLogic;
using GameServer.GameLogic.Battles;
using GameServer.GameLogic.Troops;
using GameServer.GameLogic.Utils;

namespace GameServer.Networking.Packets
{
    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

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

        private void SetBytes(byte[] data)
        {
            Write(data);
            readableBuffer = buffer.ToArray();
        }

        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        #endregion

        #region Write Data

        private void Write(byte[] value)
        {
            buffer.AddRange(value);
        }

        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        private void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            Write(value.Length);
            buffer.AddRange(Encoding.ASCII.GetBytes(value));
        }

        public void Write(VectorTwo value)
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

        public int ReadInt()
        {
            if (buffer.Count <= readPos) throw new Exception("Could not read value of type 'int'!");
            int value = BitConverter.ToInt32(readableBuffer, readPos);
            readPos += 4;
            return value;
        }

        public bool ReadBool()
        {
            if (buffer.Count <= readPos) throw new Exception("Could not read value of type 'bool'!");
            bool value = BitConverter.ToBoolean(readableBuffer, readPos);
            readPos += 1;
            return value;

        }

        public string ReadString()
        {
            try
            {
                int length = ReadInt();
                string value = Encoding.ASCII.GetString(readableBuffer, readPos, length);
                if (value.Length > 0) readPos += length;
                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        public VectorTwo ReadVector2Int()
        {
            int x = ReadInt();
            int y = ReadInt();

            return new VectorTwo(x, y);
        }

        #endregion

        private bool disposed;

        public void Dispose()
        {
            if (disposed) return;
            buffer = null;
            readableBuffer = null;
            readPos = 0;
            disposed = true;
            
            GC.SuppressFinalize(this);
        }
    }
}