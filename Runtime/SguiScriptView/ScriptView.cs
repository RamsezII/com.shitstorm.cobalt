using _ARK_;
using _COBRA_;
using _SGUI_;
using _UTIL_;
using TMPro;
using UnityEngine.UI;

namespace _COBALT_
{
    public partial class ScriptView : ArkComponent1, IHomeTexts
    {
        public SguiWindow window;
        public SguiTabController tabController;
        public ScrollRect scrollview;
        public TMP_InputField input_field;
        public TextMeshProUGUI input_lint, input_error;
        public LintTheme lint_theme = LintTheme.theme_light;

        [NJEdit]
        public bool
             use_intellisense = true,
             space_confirms_completion = false;

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            window = GetComponentInParent<SguiWindow>(true);
            tabController = GetComponentInParent<SguiTabController>(true);

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

            input_field.onValueChanged.AddListener(OnChange);
            input_field.onValidateInput += ValidateChar;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual char ValidateChar(string text, int charIndex, char addedChar)
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
        }

        protected virtual void OnChange(string text)
        {
            using BoaShell shell = new("script_view");

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

            input_lint.text = Util.ForceCharacterWrap(reader.GetLintResult());

            if (reader.sig_error == null)
                input_error.text = string.Empty;
            else
            {
                reader.LocalizeError();
                input_error.text = Util.ForceCharacterWrap(reader.sig_long_error);
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();

            input_field.onValueChanged.RemoveListener(OnChange);
            input_field.onValidateInput -= ValidateChar;

            file_path.Reset();
            file_path.Dispose();
        }
    }
}
