using _UTIL_;
using System;
using System.Collections.Generic;

namespace _COBALT_
{
    public sealed partial class Command
    {
        public readonly Dictionary<string, Command> _commands = new(StringComparer.OrdinalIgnoreCase);
        public static readonly Command root_shell = new();

        public readonly Traductions manual;
        public readonly Action<CommandLine> action;
        public readonly Action<object> pipe;
        public readonly Func<Executor, IEnumerator<CMD_STATUS>> routine;

        //--------------------------------------------------------------------------------------------------------------

        public Command(in Traductions manual = default, in Action<CommandLine> action = default, in Action<object> pipe = default, in Func<Executor, IEnumerator<CMD_STATUS>> routine = default)
        {
            this.manual = manual;
            this.action = action;
            this.pipe = pipe;
            this.routine = routine;
        }

        //--------------------------------------------------------------------------------------------------------------

        public Command AddCommand(in Command command, params string[] aliases)
        {
            if (aliases == null || aliases.Length == 0)
                throw new ArgumentException("Aliases cannot be null or empty.", nameof(aliases));
            for (int i = 0; i < aliases.Length; ++i)
                _commands.Add(aliases[i], command);
            return command;
        }
    }
}