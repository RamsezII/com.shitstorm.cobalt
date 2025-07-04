using _BOA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
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
            int pref_len = shell.current_status.prefixe_text?.Length ?? 0;
            stdin = stdin_field.text[pref_len..];
            cursor_i = stdin_field.caretPosition - pref_len;
            return !string.IsNullOrWhiteSpace(stdin);
        }

        void RefreshStdout()
        {
            stdout_field.text = shell.stdout_text;
            stdout_field.lint.text = shell.stdout_lint;

            stdout_h = stdout_field.textComponent.GetInvisibleHeight();
            stdout_field.rT.sizeDelta = new Vector2(0, stdout_h);

            ResizeStdin();
        }

        void ResizeStdin()
        {
            Rect prect = scrollview.viewport.rect;

            float stdin_h = stdin_field.textComponent.GetInvisibleHeight();

            float bottom_height = content_rT.anchoredPosition.y - stdout_h - stdin_h - offset_bottom_h + prect.height;
            stdin_h = Mathf.Max(stdin_h, prect.height);

            stdin_field.rT.sizeDelta = new(0, stdin_h);
            content_rT.sizeDelta = new(0, stdout_h + stdin_h);

            if (bottom_height < 0)
                content_rT.anchoredPosition += new Vector2(0, -bottom_height);
        }

        void ResetStdin()
        {
            stdin_field.lint.text = shell.current_status.prefixe_lint ?? string.Empty;
            string prefixe = shell.current_status.prefixe_text ?? string.Empty;
            if (!prefixe.Equals(stdin_field.text, StringComparison.Ordinal))
                stdin_field.text = prefixe;
            stdin_field.caretPosition = prefixe.Length;
            ResizeStdin();
        }

        bool CheckStdin()
        {
            string prefixe = shell.current_status.prefixe_text ?? string.Empty;
            string current = stdin_field.text;

            if (current.StartsWith(prefixe, StringComparison.Ordinal))
                return true;

            if (current.Length < prefixe.Length)
                current = prefixe;
            else
                current = prefixe + current[prefixe.Length..];

            stdin_field.text = current;

            if (stdin_field.caretPosition < prefixe.Length)
                stdin_field.caretPosition = prefixe.Length;

            return false;
        }
    }
}