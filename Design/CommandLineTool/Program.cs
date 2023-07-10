using System;
using System.Threading;

namespace CommandLineTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
                RunCommand(args);
            else
                RunInteractiveMode();
        }

        private static void RunCommand(string[] args)
        {
            var command = args[0];
            if (command == "timer")
                ExecuteTimer(int.Parse(args[1]));
            else if (command == "printtime")
                ExecutePrintTime();
            else if (command == "h")
                ExecuteHelp();
            else if (command == "help")
                ExecuteDetailedHelp(args[1]);
            else ShowUnknownCommandError(args[0]);
        }

        public static void RunInteractiveMode()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line == "exit") return;
                RunCommand(line.Split(' '));
            }
        }

        private static void ExecuteHelp()
        {
            Console.WriteLine("Available commands: timer, printtime, help, h");
        }


        private static void ShowUnknownCommandError(string command)
        {
            Console.WriteLine("Sorry. Unknown command {0}", command);
        }

        private static void ExecutePrintTime()
        {
            Console.WriteLine(DateTime.Now);
        }

        private static void ExecuteTimer(int time)
        {
            var timeout = TimeSpan.FromMilliseconds(time);
            Console.WriteLine("Waiting for " + timeout);
            Thread.Sleep(timeout);
            Console.WriteLine("Done!");
        }

        private static void ExecuteDetailedHelp(string commandName)
        {
            Console.WriteLine("");
            if (commandName == "timer")
                Console.WriteLine("timer <ms> — starts timer for <ms> milliseconds");
            else if (commandName == "printtime")
                Console.WriteLine("printtime — prints current time");
            else if (commandName == "h")
                Console.WriteLine("h — prints available commands list");
            else if (commandName == "help")
                Console.WriteLine("help <command> — prints help for <command>");
            else
            {
                ShowUnknownCommandError(commandName);
            }
        }
    }
}