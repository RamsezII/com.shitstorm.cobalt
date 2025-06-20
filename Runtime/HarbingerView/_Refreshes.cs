using System;
using _ARK_;
using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
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

            return false;
        }

        void LintStdin()
        {
            stdin_field.lint.text = shell.current_status.prefixe_lint;

            string text = stdin_field.inputfield.text[shell.current_status.prefixe_text.Length..];

            if (!string.IsNullOrWhiteSpace(text))
            {
                var reader = BoaReader.ReadLines(shell.lint_theme, false, stdin_field.inputfield.caretPosition, text);
                var signal = new BoaSignal(SIG_FLAGS_new.LINT, reader);

                shell.PropagateSignal(signal);

                string lint = reader.GetLintResult(Color.gray6);
                stdin_field.lint.text += lint;
                Debug.Log(stdin_field.lint.text, this);
            }
        }
    }
}