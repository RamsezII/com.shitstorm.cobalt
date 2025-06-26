using _COBRA_;
using _SGUI_;
using _UTIL_;
using System.IO;

namespace _COBALT_
{
    partial class CmdUI
    {
        static void Init_OpenCodium()
        {
            const string
                flag_force_create = "--create-if-null",
                flag_F = "-F";

            Command.static_domain.AddAction(
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
                    if (exe.line.TryReadArgument(out string path, out _, strict: !force, path_mode: FS_TYPES.FILE))
                        exe.args.Add(path);
                },
                action: static exe =>
                {
                    bool force = exe.opts.ContainsKey(flag_F);
                    string file_path = (string)exe.args[0];
                    file_path = exe.shell.PathCheck(file_path, PathModes.ForceFull);
                    string error = SguiNotepad.TryOpenNotepad(file_path, false, out SguiNotepad notepad);
                });

            Command.static_domain.AddAction(
                "codium",
                max_args: 1,
                args: static exe =>
                {
                    if (exe.line.TryReadArgument(out string path, out _, strict: true, path_mode: FS_TYPES.DIRECTORY))
                        exe.args.Add(path);
                },
                action: static exe =>
                {
                    string work_dir = exe.args.Count == 0
                        ? exe.GetWorkdir()
                        : exe.shell.PathCheck((string)exe.args[0], PathModes.ForceFull);

                    if (Directory.Exists(work_dir))
                        exe.error = Constrictor.TryOpenConstrictor(work_dir, false, out Constrictor constrictor);
                    else
                        exe.error = $"directory does not exists: '{work_dir}'";
                });
        }
    }
}