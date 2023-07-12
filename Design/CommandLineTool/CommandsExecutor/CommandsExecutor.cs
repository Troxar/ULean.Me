using System.Collections.Generic;
using System.IO;

namespace CommandLineTool
{
    public class CommandsExecutor : ICommandsExecutor
    {
        private readonly TextWriter _writer;
        private readonly IEnumerable<ConsoleCommand> _commands;

        public CommandsExecutor(TextWriter writer, IEnumerable<ConsoleCommand> commands)
        {
            _writer = writer;
            _commands = commands;
        }

        public void Execute(string[] args)
        {
            if (args.Length == 0)
                _writer.WriteLine("Please specify <command> as the first command line argument");

            var commandName = args[0];
            var command = _commands.FindByName(commandName);
            if (command is null)
                _writer.WriteLine("Sorry. Unknown command {0}", command);
            else
                command.Execute(args);
        }
    }
}