using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLineTool
{
    public class CommandsExecutor : ICommandsExecutor
    {
        private readonly IServiceLocator _locator;

        public CommandsExecutor(IServiceLocator locator)
        {
            _locator = locator;
        }

        public IEnumerable<ConsoleCommand> GetAvailableCommands()
        {
            return _locator.GetAll<ConsoleCommand>();
        }

        public ConsoleCommand FindCommandByName(string name)
        {
            return _locator.GetAll<ConsoleCommand>()
                .FirstOrDefault(command => command.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void Execute(string[] args)
        {
            var writer = _locator.Get<TextWriter>();
            if (args.Length == 0)
                writer.WriteLine("Please specify <command> as the first command line argument");

            var commandName = args[0];
            var command = FindCommandByName(commandName);
            if (command is null)
                writer.WriteLine("Sorry. Unknown command {0}", command);
            else
                command.Execute(args);
        }
    }
}