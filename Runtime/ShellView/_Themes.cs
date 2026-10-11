using _ARK_;
using _SGUI_.context_click;

namespace _COBALT_
{
    partial class ShellView
    {

        //----------------------------------------------------------------------------------------------------------

        void InitThemesButton(in ContextList list)
        {
            var button_themes = list.AddButton_trad(new()
            {
                french = "Thème",
                english = "Theme",
            });

            button_themes.SetupSublist(sublist =>
            {
                foreach (var theme in LintTheme.themes.Values)
                    sublist.AddButton_trad(theme.name)._button.onClick.AddListener(() =>
                    {
                        lint_theme = theme;
                        stdin_field.caretColor = theme.cursor;
                        if (stdin_field.placeholder != null)
                            stdin_field.placeholder.color = theme.placeholder;
                        OnChange();
                    });
            });
        }
    }
}
