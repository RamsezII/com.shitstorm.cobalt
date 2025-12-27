using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitShortcuts()
        {
            if (false)
                ArkShortcuts.AddShortcut(
                    shortcutName: "cobalt_newline",
                    nameof_button: "enter",
                    action: static () =>
                    {
                        foreach (var shellview in instances)
                            if (shellview.stdin_field.isFocused)
                            {
                                shellview.stdin_field.text += "\n";
                                break;
                            }
                    },
                    shift: true
                );
        }
    }
}