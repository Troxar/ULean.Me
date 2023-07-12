using System;
using System.IO;
using System.Threading;

namespace CommandLineTool
{
    public class TimerCommand : ConsoleCommand
    {
        private readonly TextWriter _writer;

        public TimerCommand(TextWriter writer)
            : base("timer", "timer <ms> — starts timer for <ms> milliseconds")
        {
            _writer = writer;
        }

        public override void Execute(string[] args)
        {
            var time = int.Parse(args[1]);
            var timeout = TimeSpan.FromMilliseconds(time);
            _writer.WriteLine("Waiting for " + timeout);
            Thread.Sleep(timeout);
            _writer.WriteLine("Done!");
        }
    }
}