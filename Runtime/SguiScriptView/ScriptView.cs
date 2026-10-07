using _ARK_;
using _SGUI_;
using _SGUI_.composer;
using _UTIL_;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace _COBALT_
{
    public sealed partial class ScriptView : SguiFrame
    {
        public ScrollRect scrollview;
        public TMP_InputField input_field;
        public TextMeshProUGUI input_lint, input_error;
        public LintTheme lint_theme = LintTheme.theme_light;

        [UField]
        public bool
             use_intellisense = true,
             space_confirms_completion = false;

        readonly ValueNotifier<CodeInterpreter> current_interpreter = new();

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            scrollview = GetComponentInChildren<ScrollRect>(true);

            input_field = scrollview.content.Find("input-field").GetComponent<TMP_InputField>();
            input_lint = scrollview.content.Find("input-field/area/lint").GetComponent<TextMeshProUGUI>();
            input_error = scrollview.content.Find("input-field/area/error").GetComponent<TextMeshProUGUI>();

            input_field.text = string.Empty;
            input_lint.text = string.Empty;

            base.Awake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

            StartFileLoading();

            current_interpreter.Value = CodeInterpreter.instances.First().Value;

            input_field.onValueChanged.AddListener(text =>
            {
                if (current_interpreter.HasNot)
                    input_lint.text = text;
                else
                {
                    current_interpreter._value.linter(text, input_field.caretPosition, lint_theme, out var lint_text, out var error);
                    input_lint.text = error ?? lint_text;
                }
            });

            input_field.onValidateInput += (text, charIndex, addedChar) =>
            {
                if (SguiCompletor.instance.toggle.Value)
                    switch (addedChar)
                    {
                        case ' ' when space_confirms_completion:
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
            };
        }
    }
}
