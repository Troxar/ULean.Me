using System;
using System.IO;

namespace CommandLineTool
{
    public class PrintTimeCommand : ConsoleCommand
    {
        private readonly TextWriter _writer;
        public PrintTimeCommand(TextWriter writer)
            : base("printtime", "printtime — prints current time")
        {
            _writer = writer;
        }

        public override void Execute(string[] args)
        {
            _writer.WriteLine(DateTime.Now);
        }
    }
}