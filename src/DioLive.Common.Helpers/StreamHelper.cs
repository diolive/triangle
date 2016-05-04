using System;
using System.IO;

namespace DioLive.Common.Helpers
{
    public class StreamHelper : IDisposable
    {
        public StreamHelper(Stream stream)
        {
            this.Stream = stream;
        }

        public Stream Stream { get; set; }

        public void WriteBit(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }

        public bool ReadBit()
        {
            return ReadByte() != 0;
        }

        public void WriteByte(byte value)
        {
            this.Stream.WriteByte(value);
        }

        public byte ReadByte()
        {
            var value = this.Stream.ReadByte();
            if (value == -1)
            {
                throw new InvalidOperationException("Stream is over");
            }

            return (byte)value;
        }

        public void WriteWord(ushort value)
        {
            WriteByte((byte)(value >> 8));
            WriteByte((byte)(value % 256));
        }

        public ushort ReadWord()
        {
            int result = 0;

            for (int i = 0; i < 2; i++)
            {
                result = (result << 8) + ReadByte();
            }

            return (ushort)result;
        }

        public void WriteInt(int value)
        {
            for (int i = 0; i < 4; i++)
            {
                WriteByte((byte)((value >> (8 * (3 - i))) % 256));
            }
        }

        public int ReadInt()
        {
            int result = 0;

            for (int i = 0; i < 4; i++)
            {
                result = (result << 8) + ReadByte();
            }

            return result;
        }

        public void WriteGuid(Guid value)
        {
            this.Stream.Write(value.ToByteArray(), 0, 16);
        }

        public Guid ReadGuid()
        {
            byte[] buffer = new byte[16];

            if (this.Stream.Read(buffer, 0, 16) < 16)
            {
                throw new InvalidOperationException("Stream is over");
            }

            return new Guid(buffer);
        }

        public void Dispose()
        {
            this.Stream.Dispose();
        }
    }
}