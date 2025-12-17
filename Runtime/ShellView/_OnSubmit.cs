using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        void OnSubmit()
        {
            if (GetStdin(out string stdin, out int cursor))
            {
                AddLine(stdin_field.text, stdin_field.lint.text);

                CodeReader reader_check = new(
                    sig_flags: SIG_FLAGS.CHECK,
                    workdir: shell.workdir._value,
                    lint_theme: lint_theme,
                    strict_syntax: false,
                    text: stdin,
                    script_path: null,
                    cursor_i: cursor
                );

                shell.OnReader(reader_check);

                if (reader_check.sig_error != null)
                {
                    reader_check.LocalizeError();
                    Debug.LogWarning(reader_check.sig_error);
                    AddLine(reader_check.sig_long_error, reader_check.sig_long_error.SetColor(Colors.orange));
                    return;
                }

                CodeReader reader_submit = new(
                    sig_flags: SIG_FLAGS.SUBMIT,
                    workdir: shell.workdir._value,
                    lint_theme: lint_theme,
                    strict_syntax: false,
                    text: stdin,
                    script_path: null,
                    cursor_i: cursor
                );

                shell.OnReader(reader_submit);

                if (reader_submit.sig_error != null)
                {
                    reader_submit.LocalizeError();
                    Debug.LogError(reader_check.sig_error);
                    AddLine(reader_submit.sig_long_error, reader_submit.sig_long_error.SetColor(Colors.orange_red));
                    return;
                }

                AddToHistory(reader_submit.text);
            }
        }
    }
}