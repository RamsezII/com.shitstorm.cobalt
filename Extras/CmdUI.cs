using _COBRA_;
using _SGUI_;
using _UTIL_;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    internal static class CmdUI
    {
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
            const string
                opt_title = "--title",
                opt_text = "--text",
                opt_ok_button = "--ok-button",
                opt_cancel_button = "--cancel-button";

            Command.cmd_root_shell.AddCommand(new(
                "show-dialog",
                args: static exe =>
                {
                    Dictionary<string, string> options = new(StringComparer.OrdinalIgnoreCase);
                    exe.args.Add(options);

                    Dictionary<string, Action<string>> onOptions = null;
                    onOptions = new(StringComparer.InvariantCultureIgnoreCase)
                    {
                        {
                            opt_title,
                            opt =>
                            {
                                exe.args.Add(opt);
                                if (exe.line.TryReadArgument(out string arg))
                                    options[opt] = arg;
                                else
                                    exe.error = $"argument manquant pour l'option '{opt}'";
                            }
                        },
                        {
                            opt_text,
                            opt =>
                            {
                                exe.args.Add(opt);
                                if (exe.line.TryReadArgument(out string arg))
                                    options[opt] = arg;
                                else
                                    exe.error = $"argument manquant pour l'option '{opt}'";
                            }
                        },
                        {
                            opt_ok_button,
                            opt =>
                            {
                                exe.args.Add(opt);
                                if (exe.line.TryReadArgument(out string arg))
                                    options[opt] = arg;
                                else
                                    exe.error = $"argument manquant pour l'option '{opt}'";
                            }
                        },
                        {
                            opt_cancel_button,
                            opt =>
                            {
                                exe.args.Add(opt);
                                if (exe.line.TryReadArgument(out string arg))
                                    options[opt] = arg;
                                else
                                    exe.error = $"argument manquant pour l'option '{opt}'";
                            }
                        }
                    };

                    if (!exe.line.TryReadOptions(exe, onOptions))
                        exe.error = $"'{exe.cmd_name}' problem reading option ({nameof(exe.line.arg_last)}: '{exe.line.arg_last}')";
                },
                routine: EShowDialog
                ));

            static IEnumerator<CMD_STATUS> EShowDialog(Command.Executor exe)
            {
                Dictionary<string, string> options = (Dictionary<string, string>)exe.args[0];
                SguiDialog dialog = SguiDialog.ShowDialog(out var routine);

                if (options.TryGetValue(opt_title, out string title))
                    dialog.trad_title.SetTrad(title);

                if (options.TryGetValue(opt_text, out string text))
                    dialog.trad_text.SetTrad(text);

                if (options.TryGetValue(opt_ok_button, out string ok_button))
                    dialog.button_yes.GetComponentInChildren<Traductable>().SetTrad(ok_button);

                if (options.TryGetValue(opt_cancel_button, out string cancel_button))
                    dialog.button_no.GetComponentInChildren<Traductable>().SetTrad(cancel_button);

                while (routine.MoveNext())
                    yield return new CMD_STATUS(CMD_STATES.BLOCKING);

                exe.Stdout(routine.Current);
            }
        }
    }
}