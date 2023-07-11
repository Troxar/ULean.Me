using System;
using System.IO;
using System.Threading;

namespace CommandLineTool
{
    public class TimerCommand : ConsoleCommand
    {
        public TimerCommand(IServiceLocator locator)
            : base("timer", "timer <ms> — starts timer for <ms> milliseconds", locator) { }

        public override void Execute(string[] args)
        {
            var time = int.Parse(args[1]);
            var timeout = TimeSpan.FromMilliseconds(time);
            var writer = _locator.Get<TextWriter>();
            writer.WriteLine("Waiting for " + timeout);
            Thread.Sleep(timeout);
            writer.WriteLine("Done!");
        }
    }
}