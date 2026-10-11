using _ARK_;
using _SGUI_.context_click;
using _UTIL_;
using UnityEngine;
using UnityEngine.UI;

namespace _COBALT_
{
    partial class ShellView
    {
        [SerializeField] LintThemeCollection lint_themes;
        [SerializeField] Graphic graphic_background;
        public readonly ValueNotifier<LintTheme> lint_theme = new();

        //----------------------------------------------------------------------------------------------------------

        void InitThemesButton(in ContextList list)
        {
            lint_theme.AddListener(theme =>
            {
                if (false)
                    graphic_background.color = theme.background;
                stdin_field.caretColor = theme.cursor;

                if (stdin_field.placeholder != null)
                    stdin_field.placeholder.color = theme.placeholder;

                OnChange();
            });

            var button_themes = list.AddButton_trad(new()
            {
                french = "Thème",
                english = "Theme",
            });

            button_themes.SetupSublist(sublist =>
            {
                foreach (var theme in lint_themes.themes)
                {
                    var button = sublist.AddButton_trad(theme.name);

                    if (theme == lint_theme._value)
                        sublist.SelectButton(button);

                    button._button.onClick.AddListener(() => lint_theme.Value = theme);
                }
            });
        }
    }
}
