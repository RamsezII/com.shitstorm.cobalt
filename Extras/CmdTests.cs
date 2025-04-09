using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    static class CobaltExtras
    {
        static Command extras;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            extras = Command.cmd_root_shell.AddCommand(new("useful"), "useless");

            extras.AddCommand(new(
                "deez",
                manual: new("HA!"),
                action: exe => exe.Stdout("nuts!")
                ));

            extras.AddCommand(new(
                "lezduit",
                manual: new("aherm.. whahh?"),
                action: exe => exe.Stdout("brah")
                ));
        }
    }
}