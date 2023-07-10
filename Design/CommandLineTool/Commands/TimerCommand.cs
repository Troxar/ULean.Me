using System;
using System.IO;
using System.Threading;

namespace CommandLineTool.Commands
{
    public static class TimerCommand
    {
        public static void Execute(int time, TextWriter writer)
        {
            var timeout = TimeSpan.FromMilliseconds(time);
            writer.WriteLine("Waiting for " + timeout);
            Thread.Sleep(timeout);
            writer.WriteLine("Done!");
        }
    }
}