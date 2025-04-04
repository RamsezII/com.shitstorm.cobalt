using _UTIL_;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _COBALT_
{
    public sealed partial class Command
    {
        public readonly Dictionary<string, Command> _commands = new(StringComparer.OrdinalIgnoreCase);
        public IEnumerable<string> ECommands_keys => _commands.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase);
        public IEnumerable<KeyValuePair<string, Command>> ECommands_pairs => _commands.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase);

        public static readonly Command cmd_root_shell = new(stay_alive: true);

        public readonly Traductions manual;
        public readonly bool stay_alive;
        public readonly Action<Executor, Line> args;
        public readonly Action<Executor> action;
        public readonly Action<Executor, string> on_stdin;
        public readonly Func<Executor, IEnumerator<CMD_STATUS>> routine;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            cmd_root_shell._commands.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------

        public Command(
            in Traductions manual = default,
            in bool stay_alive = default,
            in Action<Executor, Line> args = default,
            in Action<Executor> action = default,
            in Action<Executor, string> on_stdin = default,
            in Func<Executor, IEnumerator<CMD_STATUS>> routine = default
            )
        {
            this.manual = manual;
            this.stay_alive = stay_alive;
            this.args = args;
            this.action = action;
            this.on_stdin = on_stdin;
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

        public bool TryReadCommand(in Line line, out List<KeyValuePair<string, Command>> path)
        {
            path = new();
            return TryReadCommand_ref(line, this, path);

            static bool TryReadCommand_ref(in Line line, in Command parent, in List<KeyValuePair<string, Command>> path)
            {
                if (line.TryReadArgument(out string cmd_name, parent.ECommands_keys))
                    if (parent._commands.TryGetValue(cmd_name, out Command intermediate))
                    {
                        path.Add(new(cmd_name, intermediate));
                        TryReadCommand_ref(line, intermediate, path);
                    }
                    else
                        line.read_i = line.start_i;
                return path.Count > 0;
            }
        }
    }
}