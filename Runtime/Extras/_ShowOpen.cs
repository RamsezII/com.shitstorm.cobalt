using System.Collections.Generic;
using _COBRA_;
using _SGUI_;
using _UTIL_;

namespace _COBALT_
{
    partial class CmdUI
    {
        static void Init_ShowOpen()
        {
            const string
                flag_d = "-d",
                flag_f = "-f";

            Command.static_domain.AddRoutine(
                "show-open-prompt",
                opts: static exe =>
                {
                    if (exe.line.TryRead_one_of_the_flags(exe, out string flag, flag_d, flag_f))
                        exe.opts.Add(flag, null);
                },
                routine: ERoutine
            );

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor exe)
            {
                FS_TYPES mode = FS_TYPES.BOTH;

                if (exe.opts.ContainsKey(flag_d))
                    mode = FS_TYPES.DIRECTORY;
                if (exe.opts.ContainsKey(flag_f))
                    mode = FS_TYPES.FILE;

                bool done = false;
                SguiOpen clone = SguiOpen.InstantiatePrompt(mode, result_path =>
                {
                    if (result_path != null)
                    {
                        Command.Line line = new(string.Empty, SIG_FLAGS.EXEC, exe.shell);
                        exe.Stdout(result_path, line: line);
                    }
                    done = true;
                });

                try
                {
                    while (!done && clone != null)
                    {
                        if (exe.line.flags.HasFlag(SIG_FLAGS.KILL))
                        {
                            clone.Oblivionize();
                            clone = null;
                            done = true;
                            break;
                        }
                        yield return new CMD_STATUS(CMD_STATES.BLOCKING);
                    }
                }
                finally
                {
                    if (clone != null)
                        clone.Oblivionize();
                    clone = null;
                }
            }
        }
    }
}