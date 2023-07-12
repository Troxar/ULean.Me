using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLineTool
{
    public class HelpCommand : ConsoleCommand
    {
        private readonly TextWriter _writer;
        private readonly Lazy<IEnumerable<ConsoleCommand>> _commands;

        public HelpCommand(TextWriter writer, Lazy<IEnumerable<ConsoleCommand>> commands)
            : base("h", "h - prints available commands list")
        {
            _writer = writer;
            _commands = commands;
        }
        
        public override void Execute(string[] args)
        {
            var names = _commands.Value.Select(command => command.Name);
            _writer.WriteLine("Available commands: " + string.Join(", ", names));
        }
    }
}