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

        public LintTheme[] lint_themes = {
            LintTheme.theme_light,
            LintTheme.theme_dark,
        };

        readonly ValueNotifier<LintTheme> current_theme = new();

        //--------------------------------------------------------------------------------------------------------------

        void InitThemes()
        {
            current_theme.Value = lint_themes[0];

            var button_theme = AddTopRightButton(new()
            {
                french = "Thème",
                english = "Theme",
            });

            button_theme.onList += (ContextList list) =>
            {
                foreach (var theme in lint_themes)
                    list.AddButton_trad(theme.name)._button.onClick.AddListener(() => current_theme.Value = theme);
            };

            current_theme.AddListener(value =>
            {
                if (current_theme.HasNot)
                    button_theme.trad_label.SetTraductions(new()
                    {
                        french = "Thème",
                        english = "Theme",
                    });
                else
                {
                    graphic_background.color = current_theme._value.background;
                    button_theme.trad_label.SetTraductions(new()
                    {
                        french = $"Thème : {current_theme._value.name.french.Bold()}",
                        english = $"Theme: {current_theme._value.name.english.Bold()}",
                    });
                }

                RefreshInputField();
            });
        }
    }
}