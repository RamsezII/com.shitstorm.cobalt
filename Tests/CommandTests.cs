using _ARK_;
using System.Collections.Generic;
using UnityEngine;

namespace _COBALT_
{
    internal static class CommandTests
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command.cmd_root_shell.AddCommand(new Command(
                args: (exe, line) =>
                {
                    if (line.TryReadArgument(out string scene_name, new string[] { "scene_test1", "scene_test2", "scene_test3", }))
                        exe.args.Add(scene_name);
                },
                action: exe =>
                {
                    Debug.Log($"Loading scene: {exe.args[0]}");
                    NUCLEOR.instance.scheduler.AddRoutine(Util.EWaitForSeconds(3, false, null));
                }),
                "load-scene", "LoadScene");

            Command.cmd_root_shell.AddCommand(new Command(
                args: (exe, line) =>
                {
                    if (line.TryReadArgument(out string arg))
                        exe.args.Add(int.Parse(arg));
                },
                routine: EPipeTest),
                "pipe-test");

            static IEnumerator<CMD_STATUS> EPipeTest(Executor executor)
            {
                int loops = (int)executor.args[0];
                for (int i = 0; i < loops; ++i)
                {
                    executor.Stdout($"pipe test: '{i}'");
                    float timer = 0;
                    while (timer < 1)
                    {
                        timer += 3 * Time.deltaTime;
                        yield return new CMD_STATUS()
                        {
                            state = CMD_STATES.BLOCKING,
                            progress = (i + timer) / loops,
                        };
                    }
                }
            }
        }
    }
}