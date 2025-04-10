using _COBRA_;
using _SGUI_;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    internal static class CmdUI
    {
        enum DialogsOptions : byte
        {
            Title,
            Text,
        }

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command.cmd_root_shell.AddCommand(new("event-system")).AddCommand(new(
                "show-selected",
                action: static exe =>
                {
                    if (EventSystem.current == null)
                        exe.error = "no event system";
                    else if (EventSystem.current.currentSelectedGameObject == null)
                        exe.error = "no selected object";
                    else
                        exe.Stdout(EventSystem.current.currentSelectedGameObject.transform.GetPath(true));
                }
                ));

            Init_ShowDialog();
        }

        static void Init_ShowDialog()
        {
            Command.cmd_root_shell.AddCommand(new(
                "show-dialog",
                args: static exe =>
                {
                    Dictionary<string, Action<string>> onOptions = null;
                    onOptions = new(StringComparer.InvariantCultureIgnoreCase)
                    {
                        {
                            "--text",
                            opt =>
                            {
                                exe.args.Add(opt);
                                if (exe.line.TryReadArgument(out string arg))
                                    exe.args.Add(arg);
                                else
                                    exe.error = $"argument manquant pour l'option '{opt}'";
                            }
                        },
                    };

                    if (!exe.line.TryReadOptions(exe, onOptions))
                        exe.error = $"'{exe.cmd_name}' problem reading option ({nameof(exe.line.arg_last)}: '{exe.line.arg_last}')";
                },
                routine: EShowDialog
                ));

            static IEnumerator<CMD_STATUS> EShowDialog(Command.Executor exe)
            {
                try
                {
                    SguiDialog dialog = SguiDialog.ShowDialog(out var routine);

                    while (routine.MoveNext())
                        yield return new CMD_STATUS(CMD_STATES.BLOCKING);

                    exe.Stdout(routine.Current);
                }
                finally
                {
                }
            }
        }
    }
}