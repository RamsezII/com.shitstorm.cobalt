using _COBRA_;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    internal static partial class CmdUI
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Init_Tests();
            Init_ShowDialog();
            Init_OpenCodium();
            Init_ShowOpen();

            Command.static_domain.AddDomain(new("event-system")).AddAction(
                "show-selected",
                action: static exe =>
                {
                    if (EventSystem.current == null)
                        exe.error = "no event system";
                    else if (EventSystem.current.currentSelectedGameObject == null)
                        exe.error = "no selected object";
                    else
                        exe.Stdout(EventSystem.current.currentSelectedGameObject.transform.GetPath(true));
                },
                args: null);
        }
    }
}