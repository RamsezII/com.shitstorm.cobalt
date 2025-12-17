using _COBRA_;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        readonly Queue<LintedString> _logs = new();

        string stdout_text, stdout_lint;

        const byte max_lines = 100;

        //----------------------------------------------------------------------------------------------------------

        public void AddLine(object data, string lint = null)
        {
            string str = string.Empty;

            if (data != null)
                str = data is string s ? s : data.ToString();

            lint ??= str;

            StringBuilder
                sb_text = new(),
                sb_lint = new();

            lock (_logs)
            {
                while (_logs.Count >= max_lines)
                    _logs.Dequeue();

                _logs.Enqueue(new(str, lint));
                Debug.Log($"{this}\n{lint}", this);

                foreach (LintedString log in _logs)
                {
                    sb_text.AppendLine(log.Text);
                    sb_lint.AppendLine(log.Lint);
                }
            }

            stdout_text = sb_text.ToString();
            stdout_lint = sb_lint.ToString();

            RefreshStdout();
        }

        void RefreshStdout()
        {
            stdout_field.text = stdout_text;
            stdout_field.lint.text = stdout_lint;

            stdout_h = stdout_field.textComponent.GetInvisibleHeight();
            stdout_field.rT.sizeDelta = new Vector2(0, stdout_h);

            ResizeStdin();
        }
    }
}