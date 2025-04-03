using _ARK_;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _COBALT_
{
    internal static class CommandTests
    {


        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command.cmd_root_shell.AddCommand(new Command(
                action: line =>
                {
                    if (line.ReadArgument(out string scene_name, out _, new string[] { "scene_test1", "scene_test2", "scene_test3", }))
                        if (line.signal == CMD_SIGNAL.EXEC)
                            NUCLEOR.instance.scheduler.AddRoutine(Util.EWaitForSeconds(3, false, null));
                }),
                "load-scene", "LoadScene");

            Command.cmd_root_shell.AddCommand(new Command(
                routine: EPipeTest),
                "pipe-test");

            static IEnumerator<CMD_STATUS> EPipeTest(Executor executor)
            {
                if (executor.line.ReadArgument(out string argument, out bool isNotEmpty, Command.cmd_root_shell._commands.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase)))
                    if (executor.line.signal == CMD_SIGNAL.EXEC)
                        if (isNotEmpty)
                            if (Command.cmd_root_shell._commands.TryGetValue(argument, out Command command))
                            {
                                const int loops = 5;
                                for (int i = 0; i < loops; ++i)
                                {
                                    new Executor(executor.stack, command).Executate(new($"'{i}'", executor.line.signal));
                                    float timer = 0;
                                    while (timer < 1)
                                    {
                                        timer += 3 * Time.deltaTime;
                                        yield return new CMD_STATUS()
                                        {
                                            state = CMD_STATE.BLOCKING,
                                            progress = (i + timer) / loops,
                                        };
                                    }
                                }
                            }
                            else
                                Debug.LogWarning($"Command \"{argument}\" not found");
            }
        }
    }
}