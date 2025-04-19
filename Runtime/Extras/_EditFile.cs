using _COBRA_;
using _SGUI_;
using System.Collections.Generic;

namespace _COBALT_
{
    partial class CmdUI
    {
        static void Init_EditFile()
        {
            Command.static_domain.AddRoutine(
                "edit-file",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.line.TryReadArgument(out string path, out bool seems_valid, strict: true, path_mode: PATH_FLAGS.FILE))
                        exe.args.Add(exe.shell.PathCheck(path, PathModes.ForceFull));
                },
                routine: ERoutine);

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor exe)
            {
                SguiEditor clone = SguiWindow.InstantiateWindow<SguiEditor>();
                clone.Init((string)exe.args[0], true);

                try
                {
                    while (clone != null)
                        if (exe.line.signal.HasFlag(SIGNALS.KILL))
                        {
                            clone.sgui_toggle_window.Update(false);
                            break;
                        }
                        else
                            yield return new CMD_STATUS(CMD_STATES.BLOCKING);
                }
                finally
                {
                    if (clone != null)
                        clone.sgui_toggle_window.Update(false);
                }
            }
        }
    }
}