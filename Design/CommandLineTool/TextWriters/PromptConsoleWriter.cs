using System;
using System.IO;
using System.Text;

namespace CommandLineTool
{
    public class PromptConsoleWriter : TextWriter
    {
        private readonly string _prompt;

        public PromptConsoleWriter(string prompt) : base()
        {
            _prompt = prompt;
        }

        public override void WriteLine(string s)
        {
            Console.Out.WriteLine(_prompt + s);
        }

        public override Encoding Encoding => Console.Out.Encoding;
    }
}