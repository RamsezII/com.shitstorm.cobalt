using _COBRA_;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    internal static class CmdUI
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command cmd = Command.cmd_root_shell.AddCommand(new("event-system"));
            cmd.AddCommand(new(
                "show-selected",
                action: exe =>
                {
                    if (EventSystem.current == null)
                        exe.error = "no event system";
                    else if (EventSystem.current.currentSelectedGameObject == null)
                        exe.error = "no selected object";
                    else
                        exe.Stdout($"\"{EventSystem.current.currentSelectedGameObject.name}\" ({EventSystem.current.currentSelectedGameObject.transform.GetPath(true)})");
                }
                ));
        }
    }
}