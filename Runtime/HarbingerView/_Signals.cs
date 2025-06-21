using _ARK_;
using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        protected override void OnSelectStdin(string text)
        {
            base.OnSelectStdin(text);
            NUCLEOR.delegates.onEndOfFrame_once += () =>
            {
                int min_pos = shell.current_status.prefixe_text?.Length ?? 0;
                if (stdin_field.inputfield.caretPosition < min_pos)
                    stdin_field.inputfield.caretPosition = min_pos;
            };
        }

        protected override char OnValidateStdin_char(string text, int charIndex, char addedChar)
        {
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

        protected override void OnStdinChanged(string text)
        {
            if (shell.current_status.state != Contract.Status.States.WAIT_FOR_STDIN)
            {
                stdin_save = string.Empty;
                ResetStdin();
            }

            if (!CheckStdin())
                return;

            ResizeStdin();
            OnChange();
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
    }
}