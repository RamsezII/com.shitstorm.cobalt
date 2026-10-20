using _ARK_;
using _SGUI_.context_click;
using _UTIL_;
using UnityEngine;
using UnityEngine.UI;

namespace _COBALT_
{
    partial class ScriptView
    {
        [SerializeField] Graphic graphic_background;

        [SerializeField] LintThemeCollection lint_themes;
        [SerializeField] LintTheme initial_theme;

        readonly ValueNotifier<LintTheme> current_theme = new();

        //--------------------------------------------------------------------------------------------------------------

        void InitThemes()
        {
            var button_theme = header.AddButton(
                label: new()
                {
                    french = "Thème",
                    english = "Theme",
                },
                show_arrow: true
            );

            button_theme.onList += (ContextList list) =>
            {
                foreach (var theme in lint_themes.themes)
                    list.AddButton_trad(theme.name)._button.onClick.AddListener(() => current_theme.Value = theme);
            };

            current_theme.AddListener(value =>
            {
                if (current_theme.HasNot)
                    button_theme.label.SetTraductions(new()
                    {
                        french = "Thème",
                        english = "Theme",
                    });
                else
                {
                    graphic_background.color = current_theme._value.background;
                    button_theme.label.SetTraductions(new()
                    {
                        french = $"Thème : {current_theme._value.name.french.Bold()}",
                        english = $"Theme: {current_theme._value.name.english.Bold()}",
                    });
                }

                RefreshInputField();
            });

            current_theme.Value = initial_theme != null ? initial_theme : lint_themes.theme_light;
        }
    }
}
