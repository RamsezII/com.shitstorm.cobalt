﻿using _COBRA_;
using _SGUI_;
using _UTIL_;
using System.Collections.Generic;

namespace _COBALT_
{
    partial class CmdUI
    {
        static void Init_ShowDialog()
        {
            const string
                opt_title = "--title",
                opt_text = "--text",
                opt_ok_button = "--ok-button",
                opt_cancel_button = "--cancel-button";

            Command.static_domain.AddRoutine(
                "show-dialog",
                opts: static exe =>
                {
                    if (exe.line.TryRead_options(exe, out var output, opt_text, opt_title, opt_ok_button, opt_cancel_button))
                        foreach (var pair in output)
                            exe.opts.Add(pair.Key, pair.Value);
                    else
                        exe.error = $"'{exe.command.name}' problem reading option ({nameof(exe.line.arg_last)}: '{exe.line.arg_last}')";
                },
                routine: EShowDialog
                );

            static IEnumerator<CMD_STATUS> EShowDialog(Command.Executor exe)
            {
                SguiDialog dialog = SguiDialog.ShowDialog<SguiDialog>(out var routine);
                dialog.onDestroy += exe.line.shell.terminal.ForceSelectStdin;

                if (exe.opts.TryGetValue(opt_title, out object title))
                    dialog.trad_title.SetTrad((string)title);

                if (exe.opts.TryGetValue(opt_text, out object text))
                    dialog.SetText(new((string)text));

                if (exe.opts.TryGetValue(opt_ok_button, out object ok_button))
                    dialog.button_yes.GetComponentInChildren<Traductable>().SetTrad((string)ok_button);

                if (exe.opts.TryGetValue(opt_cancel_button, out object cancel_button))
                    dialog.button_no.GetComponentInChildren<Traductable>().SetTrad((string)cancel_button);

                while (routine.MoveNext())
                    yield return new CMD_STATUS(CMD_STATES.BLOCKING);

                exe.Stdout(routine.Current);
            }
        }
    }
}