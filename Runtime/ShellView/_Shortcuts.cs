using _ARK_;
using UnityEngine;
using _SGUI_;
using _SGUI_.composer;

namespace _COBALT_
{
    partial class ShellView
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitShortcuts()
        {
            if (OSView.instance.softwaresButtons.TryGetValue(typeof(ShellView), out var software_button))
                ArkShortcuts.AddShortcut_keyboard(
                    shortcutName: typeof(ShellView).FullName,
                    action: () =>
                    {
                        foreach (var frame in SguiFrame.instances._collection)
                            if (frame is ShellView view)
                            {
                                view.TakeFocus();
                                return;
                            }
                        software_button.InstantiateSoftware();
                    },
                    bindings: UnityEngine.InputSystem.Key.O
                );

            if (false)
                ArkShortcuts.AddShortcut_keyboard(
                    shortcutName: "cobalt_newline",
                    action: static () =>
                    {
                        foreach (var shellview in instances)
                            if (shellview.stdin_field.isFocused)
                            {
                                shellview.stdin_field.text += "\n";
                                break;
                            }
                    },
                    shift: true,
                    bindings: UnityEngine.InputSystem.Key.Enter
                );
        }
    }
}
