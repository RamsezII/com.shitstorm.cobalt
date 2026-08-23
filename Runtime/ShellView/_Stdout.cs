using _ARK_;
using _COBRA_;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        readonly Queue<LintedString> _logs = new();
        int logs_character_count;

        const byte max_lines = 100;
        const int max_buffer_characters = 32 * 1024;

        //----------------------------------------------------------------------------------------------------------

        public void AddLine(object data, string lint = null)
        {
            string str = string.Empty;

            if (data != null)
                str = data is string s ? s : data.ToString();

            lint ??= str;

            LintedString new_log = new(str, lint);

            lock (_logs)
            {
                _logs.Enqueue(new_log);
                logs_character_count += GetCharacterCost(new_log);

                while (_logs.Count > 1 && (_logs.Count > max_lines || logs_character_count > max_buffer_characters))
                    logs_character_count -= GetCharacterCost(_logs.Dequeue());

                Debug.Log($"{shell.status._value.prefixe.Lint}{lint}", this);
            }

            RefreshStdout();
        }

        static int GetCharacterCost(in LintedString log) => log.Text.Length + log.Lint.Length + 2;

        void RefreshStdout() => Util.AddActionOnce(ref NUCLEOR.delegates.LateUpdate_onEndOfFrame_once, RefreshStdout_direct);
        void RefreshStdout_direct()
        {
            StringBuilder
                sb_text = new(),
                sb_lint = new();

            lock (_logs)
            {
                foreach (LintedString log in _logs)
                {
                    sb_text.AppendLine(log.Text);
                    sb_lint.AppendLine(log.Lint);
                }
            }

            stdout_field.text = sb_text.ToString();
            stdout_field.lint.text = sb_lint.ToString();

            stdout_h = stdout_field.textComponent.GetInvisibleHeight();
            stdout_field.rT.sizeDelta = new Vector2(0, stdout_h);

            ResizeStdin();
        }
    }
}
