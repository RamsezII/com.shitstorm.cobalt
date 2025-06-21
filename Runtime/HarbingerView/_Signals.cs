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
                ResetStdin();
                goto failure;
            }

            if (!CheckStdin())
                goto failure;

            try
            {
                switch (addedChar)
                {
                    case '\t':
                        last_tab = Time.frameCount;
                        OnTab();
                        goto failure;

                    case '\n':
                        OnSubmit();
                        goto failure;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e, this);
                goto failure;
            }

            return addedChar;

        failure:
            return '\0';
        }

        protected override void OnValueChanged(string text)
        {
            if (shell.current_status.state != Contract.Status.States.WAIT_FOR_STDIN)
            {
                ResetStdin();
                return;
            }

            if (!CheckStdin())
                return;

            if (shell.current_status.state == Contract.Status.States.WAIT_FOR_STDIN)
            {
                if (Time.frameCount > last_tab)
                    stdin_save = text;
            }
            else
            {
                stdin_save = string.Empty;
                ResetStdin();
            }

            ResizeStdin();
            LintStdin();
        }

        void OnTab()
        {
            GetStdin(out string text, out int cursor_i);

            var reader = BoaReader.ReadLines(shell.lint_theme, false, cursor_i, text);
            var signal = new BoaSignal(SIG_FLAGS_new.TAB, reader);

            shell.PropagateSignal(signal);

            Debug.Log($"{reader.completions.Count} completions: {reader.completions.Join(" ")}");
        }

        void OnSubmit()
        {
            shell.AddLine(stdin_field.inputfield.text, stdin_field.lint.text);
            if (GetStdin(out string text, out int cursor_i))
            {
                var reader = BoaReader.ReadLines(shell.lint_theme, false, cursor_i, text);
                var signal = new BoaSignal(SIG_FLAGS_new.SUBMIT, reader);
                shell.PropagateSignal(signal);
            }
            ResetStdin();
        }

        void LintStdin()
        {
            stdin_field.lint.text = shell.current_status.prefixe_lint;
            if (GetStdin(out string text, out int cursor_i))
            {
                var reader = BoaReader.ReadLines(shell.lint_theme, false, cursor_i, text);
                var signal = new BoaSignal(SIG_FLAGS_new.LINT, reader);

                shell.PropagateSignal(signal);

                stdin_field.lint.text += reader.GetLintResult(Color.gray6);
            }
        }
    }
}