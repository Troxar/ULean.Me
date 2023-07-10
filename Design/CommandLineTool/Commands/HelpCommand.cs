using System;
using System.Collections.Generic;
using System.IO;

namespace CommandLineTool.Commands
{
    public class HelpCommand : ConsoleCommand
    {
        private readonly Func<IEnumerable<string>> _getCommands;

        public HelpCommand(Func<IEnumerable<string>> getCommands)
            : base("h", "h - prints available commands list")
        {
            _getCommands = getCommands;
        }
        
        public override void Execute(string[] args, TextWriter writer)
        {
            writer.WriteLine("Available commands: " + string.Join(", ", _getCommands()));
        }
    }
}