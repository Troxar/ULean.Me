using Ninject.Extensions.Conventions;
using Ninject.Modules;
using System;
using System.IO;

namespace CommandLineTool
{
    public class CommandsNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(services =>
            {
                var commandTypes = services.FromThisAssembly().Select(typeof(ConsoleCommand).IsAssignableFrom);
                commandTypes.BindAllBaseClasses().Configure(config => config.InSingletonScope());
            });
            Kernel.Bind<ICommandsExecutor>().To<CommandsExecutor>().InSingletonScope();
            Kernel.Bind<TextWriter>().To<PromptConsoleWriter>()
                .WhenInjectedInto<ConsoleCommand>()
                .InSingletonScope()
                .WithConstructorArgument("prompt", "> ");
            Kernel.Bind<TextWriter>().To<ColorTextConsoleWriter>()
                .WhenInjectedInto<CommandsExecutor>()
                .InSingletonScope()
                .WithConstructorArgument(ConsoleColor.Red);
        }
    }
}