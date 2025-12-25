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
            ArkShortcuts.AddShortcut(
                shortcutName: "save_script",
                nameof_button: "s",
                action: static () =>
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                        return;

                    ScriptView instance = EventSystem.current.currentSelectedGameObject.GetComponentInParent<ScriptView>();

                    if (instance != null)
                        instance.SaveCurrentFile();
                },
                control: true
            );
        }
    }
}