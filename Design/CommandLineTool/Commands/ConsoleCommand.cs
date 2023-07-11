namespace CommandLineTool
{
    public abstract class ConsoleCommand
    {
        public string Name { get; }
        public string Help { get; }
        protected readonly IServiceLocator _locator;

        protected ConsoleCommand(string name, string help, IServiceLocator locator)
        {
            Name = name;
            Help = help;
            _locator = locator;
        }

        public abstract void Execute(string[] args);
    }
}