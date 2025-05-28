using _COBRA_;
using _SGUI_;
using System.Collections.Generic;

namespace _COBALT_
{
    partial class CmdUI
    {
        static void Init_OpenCodium()
        {
            const string
                flag_force_create = "--create-if-null",
                flag_F = "-F";

            Command.static_domain.AddRoutine(
                "edit-file",
                min_args: 1,
                opts: static exe =>
                {
                    if (exe.line.TryRead_one_of_the_flags(exe, out string flag, flag_F, flag_force_create))
                        exe.opts.Add(flag_F, null);
                },
                args: static exe =>
                {
                    bool force = exe.opts.ContainsKey(flag_F);
                    if (exe.line.TryReadArgument(out string path, out _, strict: !force, path_mode: _UTIL_.FS_TYPES.FILE))
                        exe.args.Add(path);
                },
                routine: EEditFile);

            static IEnumerator<CMD_STATUS> EEditFile(Command.Executor exe)
            {
                bool force = exe.opts.ContainsKey(flag_F);
                string file_path = (string)exe.args[0];
                file_path = exe.shell.PathCheck(file_path, PathModes.ForceFull);

                string error = SguiNotepad.TryOpenNotepad(file_path, false, out SguiNotepad notepad);

                try
                {
                    while (notepad != null)
                    {
                        if (exe.line.signal.HasFlag(SIGNALS.KILL))
                        {
                            notepad.Oblivionize();
                            break;
                        }
                        yield return new CMD_STATUS(CMD_STATES.BLOCKING);
                    }
                }
                finally
                {
                    if (notepad != null)
                        notepad.Oblivionize();
                }
            }

            Command.static_domain.AddRoutine(
                "codium",
                opts: static exe => exe.line.TryReadOption_workdir(exe),
                routine: ECodium);

            static IEnumerator<CMD_STATUS> ECodium(Command.Executor exe)
            {
                string work_dir = exe.GetWorkdir();
                string error = Constrictor.TryOpenConstrictor(work_dir, false, out Constrictor constrictor);
                constrictor.terminal = (Terminal)exe.shell.terminal;

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