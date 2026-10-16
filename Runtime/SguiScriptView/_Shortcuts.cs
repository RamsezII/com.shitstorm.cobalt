using _ARK_;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    partial class ScriptView
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AssignShortcuts()
        {
            ArkShortcuts.AddShortcut_keyboard(
                shortcutName: "save",
                action: static () =>
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                        return;

                    ScriptView instance = EventSystem.current.currentSelectedGameObject.GetComponentInParent<ScriptView>();

                    if (instance != null)
                        instance.SaveCurrentFile();
                },
                control: true,
                bindings: UnityEngine.InputSystem.Key.S
            );

            ArkShortcuts.AddShortcut_keyboard(
                shortcutName: "save_as",
                action: static () =>
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                        return;

                    ScriptView instance = EventSystem.current.currentSelectedGameObject.GetComponentInParent<ScriptView>();

                    if (instance != null)
                        instance.SaveAs();
                },
                control: true,
                shift: true,
                bindings: UnityEngine.InputSystem.Key.S
            );
        }
    }
}