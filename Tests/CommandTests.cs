using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    internal static class CommandTests
    {


        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command cmd = Command.root_shell.AddCommand("load-scene", new Command(
                new("scene command"),
                line =>
                {
                    bool notEmpty = line.TryReadArgument(out string scene_name);
                    if (line.IsCplThis)
                        line.ComputeCompletion_tab(scene_name, new string[] { "scene_test1", "scene_test2", "scene_test3", });
                    else if (notEmpty && line.signal == CMD_SIGNAL.EXEC)
                    {
                        Debug.Log($"Load scene: {scene_name}");
                        NUCLEOR.instance.scheduler.AddRoutine(Util.EWaitForSeconds(3, false, null));
                    }
                }));

            Command.root_shell.AddCommand("LoadScene", cmd);

            cmd = Command.root_shell.AddCommand("cmd-test", new Command(
                new("test command"),
                null));

            cmd.AddCommand("arg_test1", cmd);
            cmd.AddCommand("arg_test2", cmd);

            cmd.AddCommand("arg_test3", cmd);
        }
    }
}