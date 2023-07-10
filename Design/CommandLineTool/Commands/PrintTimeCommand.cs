using System;
using System.IO;

namespace CommandLineTool.Commands
{
    public static class PrintTimeCommand
    {
        public static void Execute(TextWriter writer)
        {
            writer.WriteLine(DateTime.Now);
        }
    }
}