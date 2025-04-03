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
            Command cmd_load_scene = Command.root_shell.AddCommand("load-scene", new Command(
                new("scene command"),
                line =>
                {
                    if (line.signal == CMD_SIGNAL.EXEC)
                        NUCLEOR.instance.scheduler.AddRoutine(Util.EWaitForSeconds(3, false, null));
                }));

            Command.root_shell.AddCommand("LoadScene", cmd_load_scene);

            cmd_load_scene.AddCommand("scene_test1", cmd_load_scene);
            cmd_load_scene.AddCommand("scene_test2", cmd_load_scene);

            cmd_load_scene.AddCommand("scene_test3", cmd_load_scene);
        }
    }
}