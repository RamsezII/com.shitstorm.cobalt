using UnityEngine;

namespace _COBALT_
{
    static class CobaltExtras
    {
        static Command extras;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            extras = Command.cmd_root_shell.AddCommand(new Command(manual: new("extras")), "useful", "useless");

            InitDeez();
        }

        static void InitDeez()
        {
            extras.AddCommand(new Command(
                manual: new("HA!"),
                action: exe => exe.Stdout("nuts!")
                ),
                "deez");
        }
    }
}