using System.IO;

namespace CommandLineTool
{
    public class DetailedHelpCommand : ConsoleCommand
    {
        public DetailedHelpCommand(IServiceLocator locator)
            : base("help", "help <command> — prints help for <command>", locator) { }

        public override void Execute(string[] args)
        {
            var writer = _locator.Get<TextWriter>();
            var executor = _locator.Get<ICommandsExecutor>();
            var commandName = args[1];
            var command = executor.FindCommandByName(commandName);
            if (command is null)
                writer.WriteLine("Command not found: " + commandName);
            else
                writer.WriteLine(command.Help);
        }
    }
}