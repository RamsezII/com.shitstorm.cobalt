using _UTIL_;
using System;
using System.Linq;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        //prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{NUCLEOR.terminal_path.SetColor("#73B2D9")}$";
        public readonly ListListener<Command.Executor> executors_stack = new();

        //--------------------------------------------------------------------------------------------------------------

        void AwakeExecutors()
        {
            Command.Executor executor = new(executors_stack, Command.root_shell);
            executors_stack.AddElement(executor);

            Command.root_shell.AddCommand("help", new Command(
                new("help on whats to and hows to, nowamsayn [burp]"),
                line =>
                {
                    if (line.ReadArgument(out string argument, out bool isNotEmpty, Command.root_shell._commands.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase)))
                        if (line.signal == CMD_SIGNAL.EXEC)
                            if (isNotEmpty)
                                if (Command.root_shell._commands.TryGetValue(argument, out Command command))
                                    Debug.Log($"{argument} : \"{command.manual}\"");
                                else
                                    Debug.LogWarning($"Command \"{argument}\" not found");
                            else
                                foreach (var pair in Command.root_shell._commands)
                                    Debug.Log($"{pair.Key} : \"{pair.Value.manual}\"");
                }
            ),
            "manual");

            executors_stack._list[^1].Executate(CommandLine.EMPTY);
        }
    }
}