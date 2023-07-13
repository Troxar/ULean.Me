using Ninject;
using System;

namespace CommandLineTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var executor = CreateExecutor();
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

        private static ICommandsExecutor CreateExecutor()
        {
            var container = new StandardKernel(new CommandsNinjectModule());
            return container.Get<ICommandsExecutor>();
        }
    }
}