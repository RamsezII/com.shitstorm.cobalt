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
                float timer = 0;
                while (timer < 1)
                {
                    timer += .5f * Time.unscaledDeltaTime;
                    yield return new CMD_STATUS()
                    {
                        prefixe = "loading scene...",
                        state = CMD_STATE.BLOCKING,
                        progress = timer,
                    };
                }
            }
        }
    }
}