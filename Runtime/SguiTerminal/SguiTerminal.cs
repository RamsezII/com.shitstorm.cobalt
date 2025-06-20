using _BOA_;
using _SGUI_;
using UnityEngine;

namespace _COBALT_
{
    public sealed partial class SguiTerminal : SguiWindow1
    {
        public HarbingerView harbinger_view;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            SguiGlobal.instance.button_terminal_2.software_type = typeof(SguiTerminal);
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            sgui_softwarebutton = SguiGlobal.instance.button_terminal_2;
            harbinger_view = GetComponentInChildren<HarbingerView>();
            base.Awake();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            harbinger_view.shell.lint_theme = LintTheme.theme_dark;
            base.Start();
        }
    }
}