using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        //prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{NUCLEOR.terminal_path.SetColor("#73B2D9")}$";
        public Command.Executor executor;

        //--------------------------------------------------------------------------------------------------------------

        void AwakeShell()
        {
            Command.cmd_root_shell.AddCommand(new Command(
                manual: new("echo!"),
                args: (exe, line) =>
                {
                    if (line.TryReadArgument(out string arg))
                        exe.args.Add(arg);
                },
                action: exe => exe.Stdout((string)exe.args[0]),
                on_stdin: (exe, stdin) => Debug.Log(stdin)
                ),
                "echo");

            Command.cmd_root_shell.AddCommand(new Command(
                manual: new("Of the whats to and the hows to... nowamsayn [burp]"),
                args: (exe, line) =>
                {
                    if (Command.cmd_root_shell.TryReadCommand(line, out var path))
                        exe.args.Add(path);
                },
                action: exe =>
                {
                    if (exe.args.Count > 0)
                        Debug.Log(((List<KeyValuePair<string, Command>>)exe.args[0])[^1].Value.manual);
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

            Command.cmd_root_shell.AddCommand(new Command(
                manual: new("Of the whats to and the hows to... nowamsayn [burp]"),
                args: (exe, line) =>
                {
                    if (line.TryReadArgument(out string arg))
                        exe.args.Add(arg);
                },
                on_stdin: (exe, stdin) =>
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(stdin, (string)exe.args[0]))
                        exe.Stdout(stdin);
                }
            ),
            "grep");

            Command.cmd_root_shell.AddCommand(new Command(
                manual: new("guess what"),
                action: exe => isActive.Update(false)
                ),
                "exit");

            Command.cmd_root_shell.AddCommand(new Command(
                manual: new("quits the game... :("),
                action: exe => Application.Quit()
                ),
                "shutdown");

            Command.cmd_root_shell.AddCommand(new Command(
                manual: new("empty this terminal's logs"),
                action: exe => Application.Quit()
                ),
                "clear");

            executor = new(new() { new("shell_root", Command.cmd_root_shell), }, Command.Line.EMPTY_EXE, out _);
            executor.Executate(Command.Line.EMPTY_EXE);
        }
    }
}