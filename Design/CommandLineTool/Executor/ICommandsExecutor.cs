using System.Collections.Generic;

namespace CommandLineTool.Executor
{
    public interface ICommandsExecutor
    {
        void Execute(string[] args);
        IEnumerable<string> GetAvailableCommandNames();
    }
}
