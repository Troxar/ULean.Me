using CommandLineTool.Commands;
using CommandLineTool.Executor;
using System.IO;

namespace CommandLineTool
{
    public class CommandsExecutorBuilder
    {   
        public static ICommandsExecutor Build(TextWriter writer)
        {
            var executor = new CommandsExecutor(writer);
            executor.Register(new TimerCommand());
            executor.Register(new PrintTimeCommand());
            executor.Register(new HelpCommand(executor.GetAvailableCommandNames));
            executor.Register(new DetailedHelpCommand(executor.FindCommandByName));

            return executor;
        }
    }
}