using System;
using System.IO;

namespace CommandLineTool.Commands
{
    public class DetailedHelpCommand : ConsoleCommand
    {
        private readonly Func<string, ConsoleCommand> _getCommand;

        public DetailedHelpCommand(Func<string, ConsoleCommand> getCommand)
            : base("help", "help <command> — prints help for <command>")
        {
            _getCommand = getCommand;
        }

        public override void Execute(string[] args, TextWriter writer)
        {
            var commandName = args[1];
            var command = _getCommand(commandName);
            if (command is null)
                writer.WriteLine("Command not found: " + commandName);
            else
                writer.WriteLine(command.Help);
        }
    }
}