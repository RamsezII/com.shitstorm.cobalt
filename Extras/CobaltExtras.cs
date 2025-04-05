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
            extras = Command.cmd_root_shell.AddCommand("useful", new Command(manual: new("extras")), "useless", "extras");

            extras.AddCommand("deez", 
                new Command(
                manual: new("HA!"),
                action: exe => exe.Stdout("nuts!")
                ));

            extras.AddCommand("lezduit", 
                new Command(
                manual: new("aherm.. whahh?"),
                action: exe => exe.Stdout("brah")
                ));
        }
    }
}