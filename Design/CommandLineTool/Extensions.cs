using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineTool
{
    public static class Extensions
    {
        public static ConsoleCommand FindByName(this IEnumerable<ConsoleCommand> commands, string name)
        {
            return commands
                .FirstOrDefault(command => command.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}