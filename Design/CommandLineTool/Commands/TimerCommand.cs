using System;
using System.IO;
using System.Threading;

namespace CommandLineTool.Commands
{
    public class TimerCommand : ConsoleCommand
    {
        public TimerCommand()
            : base("timer", "timer <ms> — starts timer for <ms> milliseconds") { }

        public override void Execute(string[] args, TextWriter writer)
        {
            var time = int.Parse(args[1]);
            var timeout = TimeSpan.FromMilliseconds(time);
            writer.WriteLine("Waiting for " + timeout);
            Thread.Sleep(timeout);
            writer.WriteLine("Done!");
        }
    }
}