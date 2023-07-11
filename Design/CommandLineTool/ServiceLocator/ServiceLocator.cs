using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineTool
{
    public class ServiceLocator : IServiceLocator
    {
        private readonly Dictionary<Type, List<object>> _services = new Dictionary<Type, List<object>>();

        public void Register<TService>(TService service)
        {
            var key = typeof(TService);
            if (!_services.ContainsKey(key))
                _services.Add(key, new List<object>());
            _services[key].Add(service);
        }

        public TService Get<TService>()
        {
            var type = typeof(TService);
            if (!_services.ContainsKey(type))
                throw new InvalidOperationException("Can't resolve service " + type);
            return (TService)_services[typeof(TService)].Single();
        }

        public IEnumerable<TService> GetAll<TService>()
        {
            return _services[typeof(TService)].Cast<TService>();
        }

        public static IServiceLocator Create()
        {
            var locator = new ServiceLocator();
            locator.Register(Console.Out);
            locator.Register<ConsoleCommand>(new TimerCommand(locator));
            locator.Register<ConsoleCommand>(new PrintTimeCommand(locator));
            locator.Register<ConsoleCommand>(new HelpCommand(locator));
            locator.Register<ConsoleCommand>(new DetailedHelpCommand(locator));
            locator.Register<ICommandsExecutor>(new CommandsExecutor(locator));
            return locator;
        }
    }
}