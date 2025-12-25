using _ARK_;
using _SGUI_;
using _UTIL_;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    public partial class ScriptView : MonoBehaviour
    {
        public SguiWindow window;

        public TMP_InputField input_field;
        public TextMeshProUGUI input_lint, input_error;
        public LintTheme lint_theme = LintTheme.theme_light;

        Action<ScriptView> on_stdin_linter;

        public static readonly Dictionary<string, Action<ScriptView>> on_stdin_linters = new(StringComparer.OrdinalIgnoreCase);

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            on_stdin_linters.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            ArkMachine.AddListener(() =>
            {
                LoadSettings(true);
                NUCLEOR.delegates.OnApplicationUnfocus += () => SaveSettings(false);
                NUCLEOR.delegates.OnApplicationFocus += () => LoadSettings(false);
            });
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            window = GetComponentInParent<SguiWindow>();

            input_field = transform.Find("scroll_view/viewport/content/input-field").GetComponent<TMP_InputField>();
            input_lint = transform.Find("scroll_view/viewport/content/input-field/area/lint").GetComponent<TextMeshProUGUI>();
            input_error = transform.Find("scroll_view/viewport/content/input-field/area/error").GetComponent<TextMeshProUGUI>();

            input_field.text = string.Empty;
            input_lint.text = string.Empty;
        }


        //--------------------------------------------------------------------------------------------------------------

        private void Start()
        {
            StartFileLoading();

            input_field.onValueChanged.AddListener(OnChange);
            input_field.onValidateInput += ValidateChar;

            if (on_stdin_linters.Count == 0)
                return;

            if (on_stdin_linter == null)
            {
                var custom = SguiWindow.InstantiateWindow<SguiCustom>();
                custom.trad_title.SetTrads(new()
                {
                    french = "Choisir linter",
                    english = "Choose linter",
                });

                var dropdown = custom.AddButton<SguiCustom_Dropdown>();
                dropdown._dropdown.ClearOptions();

                dropdown._dropdown.AddOptions(on_stdin_linters.Keys.ToList());

                custom.onFunc_confirm += () =>
                {
                    string name = dropdown._dropdown.GetSelectedValue();
                    if (on_stdin_linters.TryGetValue(name, out on_stdin_linter))
                        return true;
                    SguiWindow.ShowAlert(SguiDialogs.Error, out _, new("choose a valid linter"));
                    return false;
                };

                window.onOblivion += custom.Oblivionize;
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual char ValidateChar(string text, int charIndex, char addedChar)
        {
            if (SguiCompletor.instance.toggle.Value)
                switch (addedChar)
                {
                    case ' ' when settings != null && settings.space_confirms_completion:
                    case '\n':
                    case '\t':
                        {
                            string completion = SguiCompletor.instance.GetSelectedValue();
                            if (!string.IsNullOrWhiteSpace(completion))
                            {
                                text = text[..SguiCompletor.instance.compl_start] + completion + text[SguiCompletor.instance.compl_end..];
                                input_field.text = text;
                                input_field.caretPosition = SguiCompletor.instance.compl_start + completion.Length;
                            }
                            SguiCompletor.instance.ResetIntellisense();
                        }
                        return '\0';
                }
            return addedChar;
        }

        protected virtual void OnChange(string text)
        {
            if (on_stdin_linter != null)
                on_stdin_linter(this);
            else
                input_lint.text = text.SetColor(Color.black);
        }
    }
}