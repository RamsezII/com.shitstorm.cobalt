using _UTIL_;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _COBALT_
{
    public sealed partial class Command
    {
        public readonly Dictionary<string, Command> _commands = new(StringComparer.OrdinalIgnoreCase);

        public static readonly Command
            cmd_root_shell = new(),
            cmd_echo = new(
                manual: new("echo!"),
                action: line =>
                {
                    if (line.ReadArgument(out string argument, out bool isNotEmpty))
                        if (isNotEmpty && line.signal == CMD_SIGNAL.EXEC)
                            Debug.Log(argument);
                });

        public readonly Traductions manual;
        public readonly Action<CommandLine> action;
        public readonly Func<Executor, IEnumerator<CMD_STATUS>> routine;

        //--------------------------------------------------------------------------------------------------------------

        public Command(in Traductions manual = default, in Action<CommandLine> action = default, in Func<Executor, IEnumerator<CMD_STATUS>> routine = default)
        {
            this.manual = manual;
            this.action = action;
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