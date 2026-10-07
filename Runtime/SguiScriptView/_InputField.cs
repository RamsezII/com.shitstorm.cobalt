using _SGUI_;

namespace _COBALT_
{
    partial class ScriptView
    {

        //--------------------------------------------------------------------------------------------------------------

        void StartInputField()
        {
            input_field.onValueChanged.AddListener(OnValueChanged);
            input_field.onValidateInput += OnValidateInput;
        }

        //--------------------------------------------------------------------------------------------------------------

        void AutoRefreshInputField() => OnValueChanged(input_field.text);
        void OnValueChanged(string text)
        {
            if (current_theme.Has)
            {
                graphic_background.color = current_theme._value.background;
                input_field.caretColor = current_theme._value.cursor;
            }

            if (current_interpreter.HasNot)
                input_lint.text = text;
            else
            {
                current_interpreter._value.linter(text, input_field.caretPosition, current_theme._value, out var lint_text, out var error);
                input_lint.text = lint_text;
                if (!string.IsNullOrWhiteSpace(error))
                {
                    input_error.gameObject.SetActive(true);
                    input_error.text = $"{new string(' ', text.Length)}{error}";
                }
                else
                {
                    input_error.text = string.Empty;
                    input_error.gameObject.SetActive(false);
                }
            }
        }

        char OnValidateInput(string text, int charIndex, char addedChar)
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
    }
}