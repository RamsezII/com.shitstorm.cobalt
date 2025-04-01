using _ARK_;
using System.Collections.Generic;
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
                null,
                ERoutine
            ));

            Terminal.shell.commands.Add("load-scene", Terminal.shell.commands["test"]);

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor executor)
            {
                Schedulable task = NUCLEOR.instance.scheduler.AddRoutine(Util.EWaitForSeconds(3, false, null));

                while (task.routine.Current < .3f)
                    yield return new CMD_STATUS()
                    {
                        prefixe = "loading scene...",
                        state = CMD_STATE.BLOCKING,
                    };
            }
        }
    }
}