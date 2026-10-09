using _ARK_;
using _SGUI_.composer;
using _SGUI_;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Scripting.LifecycleManagement;

namespace _COBALT_
{
    public sealed partial class SguiTerminal : SguiFrame
    {
        [AutoStaticsCleanup] static readonly List<SguiTerminal> selected_stack = new();

        public ShellView shellView;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            if (OSView.instance.softwaresButtons.TryGetValue(typeof(SguiTerminal), out var software_button))
                ArkShortcuts.AddShortcut_keyboard(
                    shortcutName: typeof(SguiTerminal).FullName,
                    action: () =>
                    {
                        foreach (var inst in instances._collection)
                            if (inst is SguiTerminal term)
                            {
                                OSView.instance.ToggleSelf(true);
                                term.TakeFocus();
                                return;
                            }
                        software_button.InstantiateSoftware();
                    },
                    bindings: Key.O
                );
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnInitialize()
        {
            base.OnInitialize();

            shellView.Initialize();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnEnable()
        {
            base.OnEnable();
            selected_stack.Remove(this);
            selected_stack.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            selected_stack.Remove(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnToggleFocus(bool has_focus)
        {
            base.OnToggleFocus(has_focus);
            if (has_focus)
                NUCLEOR.instance.routinizer.AddRoutine(Util.EWaitForFrames(2, "select stdinfield on focus", this, () =>
                {
                    if (this != null && isFocused._value) shellView.stdin_field.Select();
                }));
        }

        public override void OnResized()
        {
            base.OnResized();
            shellView.ResizeStdin();
        }
    }
}
