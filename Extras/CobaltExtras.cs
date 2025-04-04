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
            extras = Command.cmd_root_shell.AddCommand(new Command(manual: new("extras")), "useful", "useless", "extras");

            extras.AddCommand(new Command(
                manual: new("HA!"),
                action: exe => exe.Stdout("nuts!")
                ),
                "deez");

            extras.AddCommand(new Command(
                manual: new("aherm.. whahh?"),
                action: exe => exe.Stdout("brah")
                ),
                "lezduit");
        }
    }
}