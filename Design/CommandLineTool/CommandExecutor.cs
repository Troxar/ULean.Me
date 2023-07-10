using System;
using System.IO;
using CommandLineTool.Commands;

namespace CommandLineTool
{
    public static class CommandExecutor
    {
        public static void RunCommand(string[] args)
        {
            var writer = Console.Out;
            var command = args[0];
            if (command == "timer")
                TimerCommand.Execute(int.Parse(args[1]), writer);
            else if (command == "printtime")
                PrintTimeCommand.Execute(writer);
            else if (command == "h")
                HelpCommand.Execute(writer);
            else if (command == "help")
                DetailedHelpCommand.Execute(args[1], writer);
            else
                ShowUnknownCommandError(command, writer);
        }

        private static void ShowUnknownCommandError(string command, TextWriter writer)
        {
            writer.WriteLine("Sorry. Unknown command {0}", command);
        }
    }
}