using _ARK_;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        readonly Queue<(string Text, string Lint)> _logs = new();
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

            var new_log = (Text: str, Lint: lint);

            lock (_logs)
            {
                _logs.Enqueue(new_log);
                logs_character_count += GetCharacterCost(new_log);

                while (_logs.Count > 1 && (_logs.Count > max_lines || logs_character_count > max_buffer_characters))
                    logs_character_count -= GetCharacterCost(_logs.Dequeue());

                Debug.Log($"{prefixe_lint}{lint}", this);
            }

            RefreshStdout();
        }

        static int GetCharacterCost(in (string Text, string Lint) log) => log.Text.Length + log.Lint.Length + 2;

        void RefreshStdout() => Util.AddActionOnce(ref NUCLEOR.delegates.LateUpdate_onEndOfFrame_once, RefreshStdout_direct);
        void RefreshStdout_direct()
        {
            StringBuilder
                sb_text = new(),
                sb_lint = new();

            lock (_logs)
            {
                foreach (var log in _logs)
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

        sealed class ShellWriter : TextWriter
        {
            readonly StringBuilder buffer = new();
            Action<string> onLine;
            public override Encoding Encoding => Encoding.UTF8;

            public ShellWriter(Action<string> onLine) => this.onLine = onLine;

            public override void Write(char value)
            {
                if (onLine == null)
                    return;

                if (value == '\n')
                {
                    string line = buffer.ToString().TrimEnd('\r');
                    buffer.Clear();
                    onLine(line);
                }
                else
                    buffer.Append(value);
            }

            public override void Write(string value)
            {
                if (value == null)
                    return;

                foreach (char c in value)
                    Write(c);
            }

            public override void Flush()
            {
                if (onLine != null && buffer.Length > 0)
                {
                    string line = buffer.ToString();
                    buffer.Clear();
                    onLine(line);
                }
            }

            protected override void Dispose(bool disposing)
            {
                onLine = null;
                buffer.Clear();
                base.Dispose(disposing);
            }
        }
    }
}
