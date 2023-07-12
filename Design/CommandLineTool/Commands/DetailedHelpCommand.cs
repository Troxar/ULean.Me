using System;
using System.IO;
using System.Collections.Generic;

namespace CommandLineTool
{
    public class DetailedHelpCommand : ConsoleCommand
    {
        private readonly TextWriter _writer;
        private readonly Lazy<IEnumerable<ConsoleCommand>> _commands;

        public DetailedHelpCommand(TextWriter writer, Lazy<IEnumerable<ConsoleCommand>> commands)
            : base("help", "help <command> — prints help for <command>")
        {
            _writer = writer;
            _commands = commands;
        }

        public override void Execute(string[] args)
        {
            var commandName = args[1];
            var command = _commands.Value.FindByName(commandName);
            if (command is null)
                _writer.WriteLine("Command not found: " + commandName);
            else
                _writer.WriteLine(command.Help);
        }
    }
}