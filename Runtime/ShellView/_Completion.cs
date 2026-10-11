using _CMD_;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        CompletionResult last_completion;
        int completion_i, completion_cursor;
        bool applying_completion, completion_active;

        //----------------------------------------------------------------------------------------------------------

        void OnChange()
        {
            GetStdin(out string stdin, out _);
            if (!applying_completion)
            {
                completion_active = false;
            }

            stdin_field.lint.text = Util.ForceCharacterWrap(prefixe_lint + GetCommandLint(stdin, shell.Parse(stdin)));
            ResizeStdin();
        }

        string GetCommandLint(string text, ParseResult parse)
        {
            StringBuilder builder = new();
            int cursor = 0;

            foreach (var token in parse.Tokens)
            {
                if (string.IsNullOrEmpty(token.Value))
                    continue;

                int start = text.IndexOf(token.Value, cursor, StringComparison.Ordinal);
                if (start < 0)
                    continue;

                builder.Append(text[cursor..start]);
                Color color = token.Type switch
                {
                    TokenType.Command => lint_theme.functions,
                    TokenType.Option => lint_theme.options,
                    TokenType.Argument => lint_theme.argument,
                    TokenType.Directive => lint_theme.flags,
                    _ => lint_theme.command_separators,
                };
                cursor = start + token.Value.Length;
                builder.Append(text[start..cursor].SetColor(color));
            }

            builder.Append(text[cursor..]);
            return builder.ToString();
        }

        void OnTab() => CycleCompletion(1);

        void OnAlt_up_down(in KeyCode key) => CycleCompletion(key == KeyCode.UpArrow ? -1 : 1);

        void OnAlt_left_right(in KeyCode key) => CycleCompletion(key == KeyCode.LeftArrow ? -1 : 1);

        void CycleCompletion(int direction)
        {
            if (shell == null || shell.IsExecuting)
                return;

            GetStdin(out string stdin, out int cursor);
            if (!completion_active || cursor != completion_cursor)
            {
                last_completion = shell.Complete(stdin, cursor);
                completion_i = direction > 0 ? -1 : 0;
            }

            string[] suggestions = last_completion.Suggestions;
            if (suggestions == null || suggestions.Length == 0)
                return;

            completion_i = (completion_i + direction + suggestions.Length) % suggestions.Length;
            string completion = suggestions[completion_i];
            applying_completion = true;
            try
            {
                stdin_field.text = Util.ForceCharacterWrap(prefixe_text + last_completion.Text[..last_completion.Start] + completion + last_completion.Text[last_completion.End..]);
                completion_cursor = last_completion.Start + completion.Length;
                stdin_field.caretPosition = prefixe_text.Length + completion_cursor;
                completion_active = true;
            }
            finally
            {
                applying_completion = false;
            }
        }
    }
}
