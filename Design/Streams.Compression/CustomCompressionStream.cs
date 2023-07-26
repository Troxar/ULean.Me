using System;
using System.IO;

namespace Streams.Compression
{
    public class CustomCompressionStream : Stream
    {
        private readonly Stream _stream;
        private readonly bool _canRead;
        private readonly BufferedStream _bufferedStream;
        private CompressedByte _bufferedValue = CompressedByte.GetDefaultValue();

        public CustomCompressionStream(Stream stream, bool read)
        {
            _stream = stream;
            _canRead = read;
            if (read)
                _bufferedStream = new BufferedStream(stream, Constants.BufferSize);
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, buffer.Length - offset);
            var read = 0;

            while (count > 0)
            {
                if (_bufferedValue.Count == 0)
                    _bufferedValue = ReadNextValue();
                if (_bufferedValue.Count == 0)
                    break;

                var toRead = Math.Min(count, _bufferedValue.Count);
                for (int i = 0; i < toRead; i++)
                    buffer[offset + read + i] = _bufferedValue.Value;
                _bufferedValue.Count -= (byte)toRead;
                read += toRead;
                count -= toRead;
            }

            return read;
        }

        private CompressedByte ReadNextValue()
        {
            var counter = _bufferedStream.ReadByte();
            if (counter == -1)
                return CompressedByte.GetDefaultValue();
            var next = _bufferedStream.ReadByte();
            if (next == -1)
                throw new InvalidOperationException("Odd length of base stream");
            return new CompressedByte((byte)next, (byte)counter);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, buffer.Length - offset);
            if (count <= 0) return;
            var compressed = new CompressedByte(buffer[offset]);

            for (int i = 1; i < count; i++)
            {
                var next = buffer[offset + i];
                if (next == compressed.Value && compressed.Count < 255)
                    compressed.Count++;
                else
                {
                    Write(compressed);
                    compressed = new CompressedByte(next);
                }
            }

            Write(compressed);
        }

        private void Write(CompressedByte compressed)
        {
            _stream.WriteByte(compressed.Count);
            _stream.WriteByte(compressed.Value);
        }

        public override bool CanRead => _canRead;

        public override bool CanSeek => false;

        public override bool CanWrite => !_canRead;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing && _stream != null)
            //    _stream.Close();
            //base.Dispose(disposing);
        }
    }

    internal class CompressedByte
    {
        public byte Value { get; }
        public byte Count { get; set; }

        public CompressedByte(byte value, byte count = 1)
        {
            Value = value;
            Count = count;
        }

        public static CompressedByte GetDefaultValue()
        {
            return new CompressedByte(0, 0);
        }
    }

    public static class Constants
    {
        public const int BufferSize = 1024;
    }
}
