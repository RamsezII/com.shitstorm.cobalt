using _ARK_;
using _SGUI_;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _COBALT_
{
    public sealed partial class SguiTerminal : SguiWindow1
    {
        static readonly List<SguiTerminal> selected_stack = new();

        public ShellView shellView;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            selected_stack.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            var button = OSView.instance.AddSoftwareButton<SguiTerminal>(new("Terminal"));

            ArkShortcuts.AddShortcut<Keyboard>(
                shortcutName: typeof(SguiTerminal).FullName,
                nameof_button: "o",
                action: () =>
                {
                    foreach (var inst in instances._collection)
                        if (inst is SguiTerminal term)
                        {
                            OSView.instance.ToggleSelf(true);
                            term.TakeFocus();
                            return;
                        }
                    button.InstantiateSoftware();
                }
            );
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            shellView = GetComponentInChildren<ShellView>(true);
            base.OnAwake();
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

        protected override void Start()
        {
            base.Start();
            shellView.stdin_field.Select();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnTakeFocus()
        {
            base.OnTakeFocus();
            NUCLEOR.instance.sequencer_parallel.AddRoutine(Util.EWaitForFrames(2, shellView.stdin_field.Select));
        }
    }
}