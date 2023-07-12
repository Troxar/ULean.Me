using Ninject;
using System;
using System.IO;

namespace CommandLineTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var container = GetDIContainer();
            var executor = container.Get<ICommandsExecutor>();
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

        private static StandardKernel GetDIContainer()
        {
            var kernel = new StandardKernel();
            kernel.Bind<TextWriter>().ToConstant(Console.Out);
            kernel.Bind<ICommandsExecutor>().To<CommandsExecutor>().InSingletonScope();
            kernel.Bind<ConsoleCommand>().To<DetailedHelpCommand>().InSingletonScope();
            kernel.Bind<ConsoleCommand>().To<HelpCommand>().InSingletonScope();
            kernel.Bind<ConsoleCommand>().To<PrintTimeCommand>().InSingletonScope();
            kernel.Bind<ConsoleCommand>().To<TimerCommand>().InSingletonScope();

            return kernel;
        }
    }
}