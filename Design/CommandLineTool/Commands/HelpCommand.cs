using System.IO;

namespace CommandLineTool.Commands
{
    public static class HelpCommand
    {
        public static void Execute(TextWriter writer)
        {
            writer.WriteLine("Available commands: timer, printtime, help, h");
        }
    }
}
