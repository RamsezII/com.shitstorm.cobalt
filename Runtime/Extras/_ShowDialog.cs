using _COBRA_;
using _SGUI_;
using System.Collections.Generic;

namespace _COBALT_
{
    partial class CmdUI
    {
        static void Init_ShowDialog()
        {
            const string
                opt_title = "--title",
                opt_ok_button = "--ok-button",
                opt_cancel_button = "--cancel-button";

            Command.static_domain.AddRoutine(
                "show-dialog",
                max_args: 1,
                opts: static exe =>
                {
                    if (exe.signal.TryRead_options(exe, out var output, opt_title, opt_ok_button, opt_cancel_button))
                        foreach (var pair in output)
                            exe.opts.Add(pair.Key, pair.Value);
                    else
                        exe.error = $"'{exe.command.name}' problem reading option ({nameof(exe.signal.arg_last)}: '{exe.signal.arg_last}')";
                },
                args: static exe =>
                {
                    if (exe.signal.TryReadArgument(out string arg, out _))
                        exe.args.Add(arg);
                },
                routine: EShowDialog
                );

            static IEnumerator<CMD_STATUS> EShowDialog(Command.Executor exe)
            {
                SguiCustom sgui = SguiWindow.InstantiateWindow<SguiCustom>();

                sgui.onDestroy += exe.signal.shell.terminal.ForceSelectStdin;
                sgui.onAction_confirm += () => exe.Stdout(true);
                sgui.onAction_cancel += () => exe.Stdout(false);

                var alert = sgui.AddButton<SguiCustom_Alert>();

                alert.SetType(SguiCustom_Alert.DialogTypes.Dialog);

                if (exe.args.Count > 0)
                    if (exe.opts.TryGetValue((string)exe.args[0], out object text))
                        alert.SetText(new((string)text));

                if (exe.opts.TryGetValue(opt_title, out object title))
                    sgui.trad_title.SetTrad((string)title);

                if (exe.opts.TryGetValue(opt_ok_button, out object ok_button))
                    sgui.trad_confirm.SetTrad((string)ok_button);

                if (exe.opts.TryGetValue(opt_cancel_button, out object cancel_button))
                    sgui.trad_cancel.SetTrad((string)cancel_button);

                while (sgui != null && !sgui.oblivionized)
                    yield return new CMD_STATUS(CMD_STATES.BLOCKING);
            }
        }
    }
}