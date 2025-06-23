using System;
using System.Linq;
using System.Text;
using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        [SerializeField] BoaReader last_reader;
        [SerializeField] string[] last_completions_tab, last_completions_all;
        [SerializeField] int last_tab;
        [SerializeField, Range(0, ushort.MaxValue)] ushort tab_i, alt_i;

        //----------------------------------------------------------------------------------------------------------

        void OnChange()
        {
            stdin_field.lint.text = shell.current_status.prefixe_lint;

            if (GetStdin(out string text, out int cursor_i))
            {
                last_reader = BoaReader.ReadLines(shell.lint_theme, false, cursor_i, text);

                SIG_FLAGS_new flags = SIG_FLAGS_new.CHANGE;

                var signal = new BoaSignal(flags, last_reader);
                shell.PropagateSignal(signal);

                shell.AddLine($"{{ {last_reader.completion_l} }} {{ {last_reader.completion_r} }}");

                stdin_field.lint.text += last_reader.GetLintResult(Color.gray6);

                last_completions_all = last_reader.completions_v.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();

                if (Time.frameCount > last_tab)
                {
                    tab_i = 0;

                    string arg_select = string.Empty;
                    if (last_reader.cpl_end > last_reader.cpl_start)
                        arg_select = text[last_reader.cpl_start..last_reader.cpl_end];

                    last_completions_tab = last_completions_all.ECompletionMatches(arg_select).ToArray();
                }
            }
        }

        void OnTab()
        {
            last_tab = Time.frameCount;
            if (last_completions_tab == null || last_completions_tab.Length == 0)
                tab_i = 0;
            else
            {
                tab_i = (ushort)(++tab_i % last_completions_tab.Length);
                string completion = last_completions_tab[tab_i];

                StringBuilder sb = new();

                sb.Append(shell.current_status.prefixe_text);
                sb.Append(last_reader.text[..last_reader.cpl_start]);
                sb.Append(completion);
                sb.Append(last_reader.text[last_reader.cpl_end..]);

                stdin_field.text = sb.ToString();

                stdin_field.caretPosition = shell.current_status.prefixe_text.Length + last_reader.cpl_start + completion.Length;
            }
        }

        void OnAlt_up_down(in KeyCode key)
        {
            if (last_completions_all == null || last_completions_all.Length == 0)
                alt_i = 0;
            else
            {
                alt_i += (ushort)(key switch
                {
                    KeyCode.UpArrow => -1,
                    KeyCode.DownArrow => 1,
                    _ => 0,
                });

                alt_i %= (ushort)last_completions_all.Length;

                string completion = last_completions_all[alt_i];

                StringBuilder sb = new();

                sb.Append(shell.current_status.prefixe_text);
                sb.Append(last_reader.text[..last_reader.cpl_start]);
                sb.Append(completion);
                sb.Append(last_reader.text[last_reader.cpl_end..]);

                stdin_field.text = sb.ToString();

                stdin_field.caretPosition = shell.current_status.prefixe_text.Length + last_reader.cpl_start + completion.Length;
            }
        }

        void OnAlt_left_right(in KeyCode key)
        {
            string completion = key switch
            {
                KeyCode.LeftArrow => last_reader.completion_l,
                KeyCode.RightArrow => last_reader.completion_r,
                _ => null,
            };

            if (completion == null)
                return;

            StringBuilder sb = new();

            sb.Append(shell.current_status.prefixe_text);
            sb.Append(last_reader.text[..last_reader.cpl_start]);
            sb.Append(completion);
            sb.Append(last_reader.text[last_reader.cpl_end..]);

            stdin_field.text = sb.ToString();

            stdin_field.caretPosition = shell.current_status.prefixe_text.Length + last_reader.cpl_start + completion.Length;
        }
    }
}