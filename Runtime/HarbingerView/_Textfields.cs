using System;
using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        [SerializeField] byte last_status_id;

        //----------------------------------------------------------------------------------------------------------

        void OnLateUpdate()
        {
            if (last_status_id != shell.current_status.id)
            {
                CheckStdin();
                last_status_id = shell.current_status.id;
            }
        }

        bool GetStdin(out string stdin, out int cursor_i)
        {
            int pref_len = shell.current_status.prefixe_text.Length;
            stdin = stdin_field.inputfield.text[pref_len..];
            cursor_i = stdin_field.inputfield.caretPosition - pref_len;
            return !string.IsNullOrWhiteSpace(stdin);
        }

        void RefreshStdout() => Util.AddAction(ref NUCLEOR.delegates.onLateUpdate, OnRefreshStdout);
        void OnRefreshStdout()
        {
            stdout_field.inputfield.text = shell.stdout_text;
            stdout_field.lint.text = shell.stdout_lint;
            Util.AddAction(ref NUCLEOR.delegates.onStartOfFrame_once, stdout_field.AudoResize);
        }

        void ResizeStdin() => Util.AddAction(ref NUCLEOR.delegates.onLateUpdate, OnResizeStdin);
        void OnResizeStdin()
        {
            float stdin_h = stdin_field.inputfield.preferredHeight;
            float line_h = stdin_field.inputfield.textComponent.GetPreferredValues("#", stdin_field.parent_rT.rect.width, 1000).y;
        }

        void ResetStdin()
        {
            stdin_field.lint.text = shell.current_status.prefixe_lint ?? string.Empty;
            string prefixe = shell.current_status.prefixe_text ?? string.Empty;
            if (!prefixe.Equals(stdin_field.inputfield.text, StringComparison.Ordinal))
                stdin_field.inputfield.text = prefixe;
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