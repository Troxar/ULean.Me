using System.Collections.Generic;

namespace CommandLineTool
{
    public interface ICommandsExecutor
    {
        void Execute(string[] args);
        IEnumerable<ConsoleCommand> GetAvailableCommands();
        ConsoleCommand FindCommandByName(string name);
    }
}
