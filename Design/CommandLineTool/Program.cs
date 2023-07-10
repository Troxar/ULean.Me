using System;

namespace CommandLineTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
                CommandExecutor.RunCommand(args);
            else
                RunInteractiveMode();
        }

        public static void RunInteractiveMode()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line == "exit") return;
                CommandExecutor.RunCommand(line.Split(' '));
            }
        }
    }   
}