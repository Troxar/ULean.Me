using System;
using System.IO;

namespace CommandLineTool.Commands
{
    public static class DetailedHelpCommand
    {
        public static void Execute(string commandName, TextWriter writer)
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
        }
    }
}