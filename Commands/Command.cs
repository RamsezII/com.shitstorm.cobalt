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
        public readonly Func<Executor, IEnumerator<CMD_STATUS>> routine;

        //--------------------------------------------------------------------------------------------------------------

        public Command()
        {
        }

        public Command(in Traductions manual, in Action<CommandLine> action) : this(manual, action, null)
        {
        }

        public Command(in Traductions manual, in Func<Executor, IEnumerator<CMD_STATUS>> routine) : this(manual, null, routine)
        {
        }

        Command(in Traductions manual, in Action<CommandLine> action, in Func<Executor, IEnumerator<CMD_STATUS>> routine)
        {
            this.manual = manual;
            this.action = action;
            this.routine = routine;
        }

        //--------------------------------------------------------------------------------------------------------------

        public Command AddCommand(in string name, in Command command, params string[] aliases)
        {
            _commands.Add(name, command);
            for (int i = 0; i < aliases.Length; ++i)
                _commands.Add(aliases[i], command);
            return command;
        }
    }
}