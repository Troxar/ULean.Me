using Ninject;
using Ninject.Extensions.Conventions;
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
            kernel.Bind<TextWriter>().To<PromptConsoleWriter>()
                .WhenInjectedInto<ConsoleCommand>()
                .InSingletonScope()
                .WithConstructorArgument("prompt", "> ");
            kernel.Bind<TextWriter>().To<ColorTextConsoleWriter>()
                .WhenInjectedInto<CommandsExecutor>()
                .InSingletonScope()
                .WithConstructorArgument(ConsoleColor.Red);
            kernel.Bind(config => config.FromThisAssembly().SelectAllClasses().BindAllBaseClasses());
            kernel.Bind(config => config.FromThisAssembly().SelectAllClasses().BindAllInterfaces());
            
            return kernel;
        }
    }
}