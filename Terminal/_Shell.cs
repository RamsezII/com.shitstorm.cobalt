using _ARK_;
using _UTIL_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        //prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{NUCLEOR.terminal_path.SetColor("#73B2D9")}$";
        public readonly ListListener<Command.Executor> executors_stack = new();
        public static readonly Command shell = new(false, default, line => new CMD_STATUS() { prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{NUCLEOR.terminal_path.SetColor("#73B2D9")}$" });

        //--------------------------------------------------------------------------------------------------------------

        static void InitShell()
        {
            shell.commands.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------

        void AwakeShell()
        {
            Command.Executor executor = new(executors_stack, shell);
            executors_stack.AddElement(executor);

            shell.commands.Add("help", new Command(false, default, line =>
            {
                foreach (Command command in shell.commands.Values)
                    Debug.Log(command.manual.ToString());
                return new CMD_STATUS();
            }));

            executors_stack._list[^1].Executate(new CommandLine(string.Empty, CMD_SIGNAL._NONE_));
        }
    }
}