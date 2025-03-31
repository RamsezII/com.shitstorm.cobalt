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

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor executor)
            {
                float timer = 0;
                while (timer < 1)
                {
                    timer += Time.unscaledDeltaTime;
                    yield return new CMD_STATUS()
                    {
                        state = CMD_STATE.BLOCKING,
                        progress = timer,
                    };
                }
            }
        }
    }
}