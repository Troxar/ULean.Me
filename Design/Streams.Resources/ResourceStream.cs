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

        private byte[] _value = null;
        private bool _valueRead;
        private int _pointer;

        public ResourceReaderStream(Stream stream, string key)
        {
            _stream = stream;
            _key = key;
            _separator = GetDefaultSeparator();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_value is null)
                SeekValue();

            return _valueRead
                ? 0
                : ReadFieldValue(buffer, offset, count);
        }

        private void SeekValue()
        {
            bool keyFound = false;
            int i = 0;

            foreach (var segment in Segments())
            {
                if (!keyFound)
                {
                    if (i % 2 == 0 && segment.GetString() == _key)
                        keyFound = true;
                    continue;
                }

                _value = segment;
                break;
            }

            if (_value is null)
                throw new EndOfStreamException();
        }

        private IEnumerable<byte[]> Segments()
        {
            byte[] buffer = new byte[Constants.BufferSize];
            var segment = new List<byte>();
            var buf = new Queue<byte>(_separator.Length);

            while (true)
            {
                var read = _stream.Read(buffer, 0, Constants.BufferSize);
                if (read == 0) break;

                for (int i = 0; i < read; i++)
                {
                    var b = buffer[i];
                    buf.Enqueue(b);
                    if (buf.Count != _separator.Length) continue;
                    if (buf.SequenceEqual(_separator))
                    {
                        yield return segment.ToArray();
                        segment.Clear();
                        buf.Clear();
                    }
                    else
                        AddToSegmentAndCheck(segment, buf);
                }
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
            count = Math.Min(count, _value.Length - _pointer);
            for (int i = 0; i < count; i++)
                buffer[i] = _value[_pointer + i];
            _pointer += count;
            _valueRead = _pointer == _value.Length;
            return count;
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

        public static byte[] GetDefaultSeparator()
        {
            return new byte[] { 0, 1 };
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
