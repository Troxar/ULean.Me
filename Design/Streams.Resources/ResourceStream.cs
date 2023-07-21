using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Streams.Resources
{
    public class ResourceReaderStream : Stream
    {
        private readonly Stream _stream;
        private readonly string _key;
        private readonly byte[] _separator;

        private SegmentalReader<byte> _reader;

        public ResourceReaderStream(Stream stream, string key)
        {
            _stream = new BufferedStream(stream, Constants.BufferSize);
            _key = key;
            _separator = GetDefaultSeparator();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_reader is null)
                SeekValue();

            return _reader?.ValueRead ?? true
                ? 0
                : ReadFieldValue(buffer, offset, count);
        }

        private void SeekValue()
        {
            bool keyFound = false;
            int i = 0;

            foreach (var segment in Segments())
            {
                if (keyFound)
                {
                    _reader = new SegmentalReader<byte>(segment);
                    return;
                }
                keyFound = i % 2 == 0 && segment.GetString() == _key;
            }

            throw new EndOfStreamException();
        }

        private IEnumerable<byte[]> Segments()
        {
            var segment = new List<byte>();
            var buffer = new Queue<byte>(_separator.Length);

            while (true)
            {
                var b = _stream.ReadByte();
                if (b == -1)
                    break;
                buffer.Enqueue((byte)b);
                if (buffer.Count != _separator.Length)
                    continue;
                if (buffer.SequenceEqual(_separator))
                {
                    yield return segment.ToArray();
                    segment.Clear();
                    buffer.Clear();
                }
                else
                    AddToSegmentAndCheck(segment, buffer);
            }
        }

        private static void AddToSegmentAndCheck(List<byte> segment, Queue<byte> buffer)
        {
            var v = buffer.Dequeue();
            segment.Add(v);

            if (v == 0 && buffer.Peek() == 0)
                buffer.Dequeue();
        }

        private int ReadFieldValue(byte[] buffer, int offset, int count)
        {
            if (_reader is null)
                throw new InvalidOperationException("Reader not configured");

            var segment = _reader.Read(count);
            Array.Copy(segment.ToArray(), 0, buffer, offset, segment.Count);

            return segment.Count;
        }

        public override void Flush()
        {
            // nothing to do
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

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
            if (disposing && _stream != null)
                _stream.Close();
            base.Dispose(disposing);
        }

        public static byte[] GetDefaultSeparator()
        {
            return new byte[] { 0, 1 };
        }
    }

    public class SegmentalReader<T>
    {
        public bool ValueRead { get; private set; }

        private readonly T[] _value;
        private readonly IEnumerator<ArraySegment<T>> _enumerator;
        private int _countToRead;

        public SegmentalReader(T[] value)
        {
            _value = value;
            _enumerator = Segments().GetEnumerator();
        }

        public ArraySegment<T> Read(int count)
        {
            _countToRead = count;
            return !ValueRead && _enumerator.MoveNext()
                ? _enumerator.Current
                : new ArraySegment<T>();
        }

        private IEnumerable<ArraySegment<T>> Segments()
        {
            var pointer = 0;

            while (!ValueRead)
            {
                var length = Math.Min(_countToRead, _value.Length - pointer);
                yield return new ArraySegment<T>(_value, pointer, length);

                pointer += length;
                ValueRead = pointer == _value.Length;
            }
        }
    }

    public static class Extensions
    {
        public static string GetString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
