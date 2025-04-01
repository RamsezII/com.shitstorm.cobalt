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
            Terminal.shell.commands.Add("test", new Command(
                false,
                new("test command"),
                line => NUCLEOR.instance.scheduler.AddRoutine(Util.EWaitForSeconds(3, false, null)),
                null
            ));

            Terminal.shell.commands.Add("load-scene", Terminal.shell.commands["test"]);
        }
    }
}