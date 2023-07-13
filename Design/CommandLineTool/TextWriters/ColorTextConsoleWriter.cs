using System;
using System.IO;
using System.Text;

namespace CommandLineTool
{
    public class ColorTextConsoleWriter : TextWriter
    {
        private readonly ConsoleColor _color;

        public ColorTextConsoleWriter(ConsoleColor color) : base()
        {
            _color = color;
        }

        public override void Write(char value)
        {
            var prev = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = _color;
                Console.Out.Write(value);
            }
            finally
            {
                Console.ForegroundColor = prev;
            }
        }

        public override Encoding Encoding => Console.Out.Encoding;
    }
}