using CommandLineTool.Executor;
using System;

namespace CommandLineTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var executor = CommandsExecutorBuilder.Build(Console.Out);
            if (args.Length > 0)
                executor.Execute(args);
            else
                RunInteractiveMode(executor);
        }

        public static void RunInteractiveMode(ICommandsExecutor executor)
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line == "exit") return;
                executor.Execute(line.Split(' '));
            }
        }
    }   
}