using _COBRA_;
using _SGUI_;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    internal static class CmdUI
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command.cmd_root_shell.AddCommand(new("event-system")).AddCommand(new(
                "show-selected",
                action: static exe =>
                {
                    if (EventSystem.current == null)
                        exe.error = "no event system";
                    else if (EventSystem.current.currentSelectedGameObject == null)
                        exe.error = "no selected object";
                    else
                        exe.Stdout($"\"{EventSystem.current.currentSelectedGameObject.name}\" ({EventSystem.current.currentSelectedGameObject.transform.GetPath(true)})");
                }
                ));

            Command cmd_sgui = Command.cmd_root_shell.AddCommand(new("sgui"));

            cmd_sgui.AddCommand(new(
                "show-dialog",
                args: static exe =>
                {

                },
                routine: EShowDialog
                ));

            static IEnumerator<CMD_STATUS> EShowDialog(Command.Executor exe)
            {
                try
                {
                    SguiDialog dialog = SguiDialog.ShowDialog(out var routine);

                    while (routine.MoveNext())
                        yield return new CMD_STATUS(CMD_STATES.BLOCKING);

                    exe.Stdout(routine.Current);
                }
                finally
                {
                }
            }
        }
    }
}