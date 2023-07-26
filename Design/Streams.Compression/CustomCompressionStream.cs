using System;
using System.IO;

namespace Streams.Compression
{
    public class CustomCompressionStream : Stream
    {
        private readonly Stream _stream;

        public CustomCompressionStream(Stream stream, bool read)
        {
            if (read)
                _stream = new CompressionStreamReader(stream);
            else
                _stream = new CompressionStreamWriter(stream);
        }

        public override void Flush() => _stream.Flush();

        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);

        public override void SetLength(long value) => _stream.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);

        public override bool CanRead => _stream.CanRead;

        public override bool CanSeek => _stream.CanSeek;

        public override bool CanWrite => _stream.CanWrite;

        public override long Length => _stream.Length;

        public override long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing && _stream != null)
            //    _stream.Close();
            //base.Dispose(disposing);
        }
    }

    internal class CompressionStreamWriter : Stream
    {
        private readonly Stream _stream;

        public CompressionStreamWriter(Stream stream)
        {
            _stream = stream;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, buffer.Length - offset);
            if (count <= 0)
                return;

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

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

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

    internal class CompressionStreamReader : Stream
    {
        private readonly BufferedStream _stream;
        private CompressedByte _compressed = CompressedByte.GetDefaultValue();

        public CompressionStreamReader(Stream stream)
        {
            _stream = new BufferedStream(stream, Constants.BufferSize);
        }

        public override void Flush() => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, buffer.Length - offset);
            var read = 0;

            while (count > 0)
            {
                if (_compressed.Count == 0)
                    _compressed = ReadNext();
                if (_compressed.Count == 0)
                    break;

                var toRead = Math.Min(count, _compressed.Count);
                for (int i = 0; i < toRead; i++)
                    buffer[offset + read + i] = _compressed.Value;
                _compressed.Count -= (byte)toRead;
                read += toRead;
                count -= toRead;
            }

            return read;
        }

        private CompressedByte ReadNext()
        {
            var count = _stream.ReadByte();
            if (count == -1)
                return CompressedByte.GetDefaultValue();

            var value = _stream.ReadByte();
            if (value == -1)
                throw new InvalidOperationException("Odd length of base stream");

            return new CompressedByte((byte)value, (byte)count);
        }

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

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
