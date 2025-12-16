using _ARK_;
using _SGUI_;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _COBALT_
{
    public sealed partial class SguiTerminal : SguiWindow1
    {
        public ShellView shellView;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            var button = OSView.instance.AddSoftwareButton<SguiTerminal>(new("Terminal"));

            ArkShortcuts.AddShortcut<Keyboard>(
                shortcutName: typeof(SguiTerminal).FullName,
                nameof_button: "o",
                action: () => OSView.instance.softwaresButtons[typeof(SguiTerminal)].InstantiateSoftware()
            );
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            shellView = GetComponentInChildren<ShellView>(true);
            base.OnAwake();
        }
    }
}