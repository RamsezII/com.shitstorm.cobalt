using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        protected override char OnValidateInput(string text, int charIndex, char addedChar)
        {
            last_input = Time.frameCount;

            if (shell.current_status.state != Contract.Status.States.WAIT_FOR_STDIN)
            {
                stdin_field.ResetTexts();
                return '\0';
            }

            if (!CheckStdin())
            {
                Debug.Log($"wrong input {{{text}}}", this);
                return '\0';
            }

            try
            {
                switch (addedChar)
                {
                    case '\t':
                        last_tab = Time.frameCount;
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

        protected override void OnValueChanged(string text)
        {
            if (shell.current_status.state != Contract.Status.States.WAIT_FOR_STDIN)
            {
                stdin_field.ResetTexts();
                return;
            }

            if (!CheckStdin())
            {
                Debug.Log($"wrong change {{{text}}}", this);
                return;
            }

            ResizeStdin();

            if (!string.IsNullOrEmpty(text) && shell.current_status.state == Contract.Status.States.WAIT_FOR_STDIN)
            {
                if (Time.frameCount > last_tab)
                    stdin_save = text;
                LintStdin();
            }
            else
            {
                stdin_save = string.Empty;
                stdin_field.ResetTexts();
            }
        }

        void OnTab(in string text, in int charIndex)
        {
            var reader = BoaReader.ReadLines(shell.lint_theme, false, charIndex, text);
            var signal = new BoaSignal(SIG_FLAGS_new.TAB, reader);

            shell.PropagateSignal(signal);

            Debug.Log($"{reader.completions.Count} completions: {reader.completions.Join(" ")}");
        }

        void OnSubmit(in string text)
        {
            Debug.Log("submit");

            stdin_field.ResetTexts();

            var reader = BoaReader.ReadLines(shell.lint_theme, false, stdin_field.inputfield.caretPosition, text);
            var signal = new BoaSignal(SIG_FLAGS_new.SUBMIT, reader);

            shell.PropagateSignal(signal);

            if (reader.error != null)
                Debug.LogError(reader.error);
        }
    }
}