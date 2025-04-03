using _UTIL_;
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
                default,
                line =>
                {
                    foreach (var pair in Command.root_shell._commands)
                        Debug.Log($"{pair.Key} : \"{pair.Value.manual}\"");
                }
            ));

            executors_stack._list[^1].Executate(CommandLine.EMPTY);
        }
    }
}