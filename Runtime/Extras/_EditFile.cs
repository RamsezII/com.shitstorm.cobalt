using _ARK_;
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
                "codium",
                opts: static exe => exe.line.TryReadOption_workdir(exe),
                routine: ERoutine);

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor exe)
            {
                string folder_path = (string)exe.args[0];
                folder_path = exe.shell.PathCheck(folder_path, PathModes.ForceFull);

                string error = Constrictor.TryOpenConstrictor(folder_path, false, out Constrictor constrictor);

                try
                {
                    while (constrictor != null)
                    {
                        if (exe.line.signal.HasFlag(SIGNALS.KILL))
                        {
                            constrictor.Oblivionize();
                            break;
                        }
                        yield return new CMD_STATUS(CMD_STATES.BLOCKING);
                    }
                }
                finally
                {
                    if (constrictor != null)
                        constrictor.Oblivionize();
                }
            }
        }
    }
}