using _BOA_;

namespace _COBALT_
{
    public sealed partial class SguiTerminal : _SGUI_.SguiTerminal
    {
        public HarbingerView harbinger_view;

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
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