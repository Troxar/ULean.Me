using System.IO;
using System.Linq;

namespace CommandLineTool
{
    public class HelpCommand : ConsoleCommand
    {
        public HelpCommand(IServiceLocator locator)
            : base("h", "h - prints available commands list", locator) { }
        
        public override void Execute(string[] args)
        {
            var writer = _locator.Get<TextWriter>();
            var executor = _locator.Get<ICommandsExecutor>();
            var names = executor.GetAvailableCommands().Select(command => command.Name);
            writer.WriteLine("Available commands: " + string.Join(", ", names));
        }
    }
}