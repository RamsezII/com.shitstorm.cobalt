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
                shortcutName: "code_actions",
                action: static () =>
                {
                    var selected = EventSystem.current?.currentSelectedGameObject;
                    var instance = selected == null ? null : selected.GetComponentInParent<ScriptView>();
                    if (instance != null && instance.input_field.isFocused)
                        instance.ShowCodeActions();
                },
                control: true,
                bindings: UnityEngine.InputSystem.Key.Period
            );

            ArkShortcuts.AddShortcut_keyboard(
                shortcutName: "complete_code",
                action: static () =>
                {
                    var selected = EventSystem.current?.currentSelectedGameObject;
                    var instance = selected == null ? null : selected.GetComponentInParent<ScriptView>();
                    if (instance != null && instance.input_field.isFocused)
                        instance.RefreshCompletions(force: true);
                },
                control: true,
                bindings: UnityEngine.InputSystem.Key.Space
            );

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

            ArkShortcuts.AddShortcut_keyboard(
                shortcutName: "format_prefix",
                action: static () =>
                {
                    var selected = EventSystem.current?.currentSelectedGameObject;
                    var instance = selected == null ? null : selected.GetComponentInParent<ScriptView>();
                    if (instance != null && instance.input_field.isFocused)
                        instance.formatShortcutStartedAt = Time.unscaledTime;
                },
                control: true,
                bindings: UnityEngine.InputSystem.Key.K
            );

            ArkShortcuts.AddShortcut_keyboard(
                shortcutName: "format_document",
                action: static () =>
                {
                    var selected = EventSystem.current?.currentSelectedGameObject;
                    var instance = selected == null ? null : selected.GetComponentInParent<ScriptView>();
                    if (instance == null)
                        return;

                    float elapsed = Time.unscaledTime - instance.formatShortcutStartedAt;
                    instance.formatShortcutStartedAt = float.NegativeInfinity;
                    if (instance.input_field.isFocused && elapsed >= 0 && elapsed <= 2f)
                        instance.FormatDocument();
                },
                control: true,
                bindings: UnityEngine.InputSystem.Key.D
            );
        }
    }
}
