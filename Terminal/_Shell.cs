using _UTIL_;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        //prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{NUCLEOR.terminal_path.SetColor("#73B2D9")}$";
        public readonly ListListener<Executor> executors_stack = new();

        //--------------------------------------------------------------------------------------------------------------

        void AwakeExecutors()
        {
            Executor executor = new(executors_stack, Command.cmd_root_shell);
            executors_stack.AddElement(executor);

            Command.cmd_root_shell.AddCommand(Command.cmd_echo, "echo");

            Command.cmd_root_shell.AddCommand(new Command(
                new("Of the whats to and the hows to... nowamsayn [burp]"),
                line =>
                {
                    if (line.ReadArgument(out string argument, out bool isNotEmpty, Command.cmd_root_shell._commands.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase)))
                        if (line.signal == CMD_SIGNAL.EXEC)
                            if (isNotEmpty)
                                if (Command.cmd_root_shell._commands.TryGetValue(argument, out Command command))
                                    Debug.Log(command.manual);
                                else
                                    Debug.LogWarning($"Command \"{argument}\" not found");
                            else
                            {
                                var groupedByValue = Command.cmd_root_shell._commands.GroupBy(pair => pair.Value);
                                foreach (var group in groupedByValue)
                                {
                                    StringBuilder sb = new();
                                    foreach (var pair in group)
                                        sb.Append($"{pair.Key}, ");

                                    sb.Remove(sb.Length - 2, 2);
                                    Debug.Log($"{sb}: {group.Key.manual}");
                                }
                            }
                }
            ),
            "help", "manual");

            executors_stack._list[^1].Executate(CommandLine.EMPTY);
        }
    }
}