using CommandLineTool.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLineTool.Executor
{
    public class CommandsExecutor : ICommandsExecutor
    {
        private readonly List<ConsoleCommand> _commands = new List<ConsoleCommand>();
        private readonly TextWriter _writer;

        public CommandsExecutor(TextWriter writer)
        {
            _writer = writer;
        }

        public void Register(ConsoleCommand command)
        {
            _commands.Add(command);
        }

        public IEnumerable<string> GetAvailableCommandNames()
        {
            return _commands.Select(command => command.Name);
        }

        public ConsoleCommand FindCommandByName(string name)
        {
            return _commands.FirstOrDefault(command => command.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void Execute(string[] args)
        {
            if (args.Length == 0)
                ShowInvalidArgsLengthError();

            var commandName = args[0];
            var command = FindCommandByName(commandName);
            if (command is null)
                ShowUnknownCommandError(commandName);
            else
                command.Execute(args, _writer);
        }

        private void ShowUnknownCommandError(string command)
        {
            _writer.WriteLine("Sorry. Unknown command {0}", command);
        }

        private void ShowInvalidArgsLengthError()
        {
            _writer.WriteLine("Please specify <command> as the first command line argument");
        }
    }
}