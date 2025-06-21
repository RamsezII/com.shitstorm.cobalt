using System;
using System.Linq;
using System.Text;
using _ARK_;
using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        [SerializeField] BoaReader last_reader;
        [SerializeField] string[] last_completions;
        [SerializeField] string stdin_save;
        [SerializeField] int last_tab, tab_i;

        //----------------------------------------------------------------------------------------------------------

        void OnChange()
        {
            stdin_field.lint.text = shell.current_status.prefixe_lint;

            if (GetStdin(out string text, out int cursor_i))
            {
                var reader = BoaReader.ReadLines(shell.lint_theme, false, cursor_i, text);

                SIG_FLAGS_new flags = SIG_FLAGS_new.CHANGE;


                var signal = new BoaSignal(flags, reader);
                shell.PropagateSignal(signal);

                stdin_field.lint.text += reader.GetLintResult(Color.gray6);

                if (Time.frameCount > last_tab)
                {
                    tab_i = 0;
                    last_reader = reader;
                    last_completions = last_reader.completions.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
                }
            }

            if (Time.frameCount > last_tab)
                stdin_save = text;
        }

        void OnTab()
        {
            last_tab = Time.frameCount;
            if (last_completions == null)
                tab_i = 0;
            else
            {
                tab_i = ++tab_i % last_completions.Length;
                string completion = last_completions[tab_i];

                StringBuilder sb = new();

                sb.Append(shell.current_status.prefixe_text);
                sb.Append(last_reader.text[..last_reader.cpl_start]);
                sb.Append(completion);
                sb.Append(last_reader.text[last_reader.cpl_end..]);

                stdin_field.inputfield.text = sb.ToString();

                stdin_field.inputfield.caretPosition = shell.current_status.prefixe_text.Length + last_reader.cpl_start + completion.Length;
            }
        }
    }
}