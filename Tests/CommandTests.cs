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
            Command.root_shell.AddCommand(new Command(
                action: line =>
                {
                    if (line.ReadArgument(out string scene_name, out _, new string[] { "scene_test1", "scene_test2", "scene_test3", }))
                        if (line.signal == CMD_SIGNAL.EXEC)
                            NUCLEOR.instance.scheduler.AddRoutine(Util.EWaitForSeconds(3, false, null));
                }),
                "load-scene", "LoadScene");

            Command.root_shell.AddCommand(new Command(
                routine: EPipeTest),
                "pipe-test");

            Command cmd_echo = Command.root_shell.AddCommand(new Command(
                new("echo"),
                line =>
                {
                    if (line.ReadArgument(out string argument, out bool isNotEmpty))
                        if (isNotEmpty)
                            Debug.Log(argument);
                },
                data => Debug.Log(data)
                ),
                "echo");

            static IEnumerator<CMD_STATUS> EPipeTest(Command.Executor executor)
            {
                if (executor.line.ReadArgument(out string argument, out bool isNotEmpty, Command.root_shell._commands.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase)))
                    if (executor.line.signal == CMD_SIGNAL.EXEC)
                        if (isNotEmpty)
                            if (Command.root_shell._commands.TryGetValue(argument, out Command command))
                                if (command.pipe == null)
                                    Debug.LogWarning($"Command \"{argument}\" does not take pipes");
                                else
                                    for (int i = 0; i < 5; ++i)
                                    {
                                        float timer = 0;
                                        while (timer < 1)
                                        {
                                            timer += 5 * Time.deltaTime;
                                            yield return new CMD_STATUS()
                                            {
                                                state = CMD_STATE.BLOCKING,
                                                progress = timer + i * .2f,
                                            };
                                        }
                                        command.pipe($"pipe test: '{i}'");
                                    }
                            else
                                Debug.LogWarning($"Command \"{argument}\" not found");
            }
        }
    }
}