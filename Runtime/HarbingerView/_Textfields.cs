using _BOA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        [SerializeField] Contract.Status last_status;
        [SerializeField] float stdin_h, stdout_h;

        //----------------------------------------------------------------------------------------------------------

        void OnLateUpdate()
        {
            if (!last_status.Equals(shell.current_status))
            {
                if (shell.current_status.state != Contract.Status.States.WAIT_FOR_STDIN)
                    tmp_progress.text = shell.current_status.progress.PercentLog(1);
                else
                    tmp_progress.text = string.Empty;

                ResetStdin();
                last_status = shell.current_status;
            }
        }

        bool GetStdin(out string stdin, out int cursor_i)
        {
            int pref_len = shell.current_status.prefixe_text.Length;
            stdin = stdin_field.inputfield.text[pref_len..];
            cursor_i = stdin_field.inputfield.caretPosition - pref_len;
            return !string.IsNullOrWhiteSpace(stdin);
        }

        void RefreshStdout()
        {
            stdout_field.inputfield.text = shell.stdout_text;
            stdout_field.lint.text = shell.stdout_lint;

            stdout_h = 0;
            if (!string.IsNullOrWhiteSpace(shell.stdout_text))
                stdout_h = stdout_field.inputfield.textComponent.GetPreferredValues(shell.stdout_text, stdout_field.parent_rT.rect.width, 1000).y;

            stdout_field.rT.sizeDelta = new Vector2(0, stdout_h);
        }

        void ResizeStdin()
        {
            Rect prect = stdin_field.parent_rT.rect;

            float line_h = stdin_field.inputfield.textComponent.GetPreferredValues("#", prect.width, 1000).y;

            float stdin_h = line_h;
            if (!string.IsNullOrWhiteSpace(stdin_field.inputfield.text))
                stdin_h = stdin_field.inputfield.textComponent.GetPreferredValues(stdin_field.inputfield.text, prect.width, 1000).y;

            stdin_h = Mathf.Max(stdin_h, prect.height);

            stdin_field.rT.sizeDelta = new(0, stdin_h);

            scrollview_rT.sizeDelta = new(0, stdout_h + stdin_h);
        }

        void ResetStdin()
        {
            stdin_field.lint.text = shell.current_status.prefixe_lint ?? string.Empty;
            string prefixe = shell.current_status.prefixe_text ?? string.Empty;
            if (!prefixe.Equals(stdin_field.inputfield.text, StringComparison.Ordinal))
                stdin_field.inputfield.text = prefixe;
            stdin_field.inputfield.caretPosition = prefixe.Length;
        }

        bool CheckStdin()
        {
            string prefixe = shell.current_status.prefixe_text ?? string.Empty;
            string current = stdin_field.inputfield.text;

            if (current.StartsWith(prefixe, StringComparison.Ordinal))
                return true;

            if (current.Length < prefixe.Length)
                current = prefixe;
            else
                current = prefixe + current[prefixe.Length..];

            stdin_field.inputfield.text = current;

            if (stdin_field.inputfield.caretPosition < prefixe.Length)
                stdin_field.inputfield.caretPosition = prefixe.Length;

            return false;
        }
    }
}