using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        protected override char OnValidateInput(string text, int charIndex, char addedChar)
        {
            if (shell.current_status.state != Contract.Status.States.WAIT_FOR_STDIN)
            {
                std_in.ResetText();
                return '\0';
            }

            try
            {
                switch (addedChar)
                {
                    case '\t':
                        frame_tab = Time.frameCount;
                        OnTab(text, charIndex);
                        return '\0';

                    case '\n':
                        OnSubmit(text);
                        return '\0';
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e, this);
                return '\0';
            }

            return addedChar;
        }

        protected override void OnValueChanged(string value)
        {
            if (!string.IsNullOrEmpty(value) && shell.current_status.state == Contract.Status.States.WAIT_FOR_STDIN)
            {
                if (Time.frameCount > frame_tab)
                    stdin_save = value;
                LintStdin(value);
            }
            else
            {
                stdin_save = string.Empty;
                std_in.ResetText();
            }
        }

        void OnTab(in string text, in int charIndex)
        {
            var reader = BoaReader.ReadLines(shell.lint_theme, false, charIndex, text);
            var signal = new BoaSignal(SIG_FLAGS_new.TAB, reader);

            shell.PropagateSignal(signal);

            Debug.Log($"{reader.completions.Count} completions: {reader.completions.Join(" ")}");
        }

        void LintStdin(in string stdin)
        {
            var reader = BoaReader.ReadLines(shell.lint_theme, false, std_in.inputfield.caretPosition, stdin);
            var signal = new BoaSignal(SIG_FLAGS_new.LINT, reader);

            shell.PropagateSignal(signal);

            std_in.lint.text = reader.GetLintResult(Color.gray6);
        }

        void OnSubmit(in string text)
        {
            Debug.Log("submit");

            std_in.ResetText();

            var reader = BoaReader.ReadLines(shell.lint_theme, false, std_in.inputfield.caretPosition, text);
            var signal = new BoaSignal(SIG_FLAGS_new.SUBMIT, reader);

            shell.PropagateSignal(signal);

            if (reader.error != null)
                Debug.LogError(reader.error);
        }
    }
}