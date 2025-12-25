using _ARK_;
using _COBRA_;
using _SGUI_;
using _UTIL_;
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
            Shell shell = new BoaShell();

            CodeReader reader = new(
                sig_flags: SIG_FLAGS.CHANGE | SIG_FLAGS.LINT,
                workdir: shell.workdir._value,
                lint_theme: lint_theme,
                strict_syntax: false,
                text: text,
                script_path: null,
                cursor_i: input_field.caretPosition
            );

            shell.OnReader(reader);

            input_lint.text = Util.ForceCharacterWrap(shell.status._value.prefixe.Lint + reader.GetLintResult());
        }
    }
}