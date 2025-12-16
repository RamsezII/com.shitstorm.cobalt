using _COBRA_;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        [SerializeField] CodeReader last_reader;
        [SerializeField] string[] last_completions_tab, last_completions_all;
        [SerializeField] int last_tab;
        [SerializeField, Range(0, ushort.MaxValue)] ushort tab_i, alt_i;

        //----------------------------------------------------------------------------------------------------------

        void OnChange()
        {
            if (GetStdin(out string stdin, out int cursor))
            {
                last_reader = new(
                    sig_flags: SIG_FLAGS.CHANGE | SIG_FLAGS.LINT,
                    workdir: shell.workdir._value,
                    lint_theme: lint_theme,
                    strict_syntax: false,
                    text: stdin,
                    script_path: null,
                    cursor_i: cursor
                );

                shell.OnReader(last_reader);

                stdin_field.lint.text = Util.ForceCharacterWrap(shell.prefixe._value.lint + last_reader.GetLintResult());

                last_completions_all = last_reader.completions_v.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();

                if (Time.frameCount > last_tab)
                {
                    tab_i = 0;

                    string arg_select = string.Empty;
                    if (last_reader.cpl_end > last_reader.cpl_start)
                        arg_select = stdin[last_reader.cpl_start..last_reader.cpl_end];

                    last_completions_tab = last_completions_all.ECompletionMatches(arg_select).ToArray();
                }
            }
            else
                ResetStdin();

            ResizeStdin();
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

                sb.Append(shell.prefixe._value.text);
                sb.Append(last_reader.text[..last_reader.cpl_start]);
                sb.Append(completion);
                sb.Append(last_reader.text[last_reader.cpl_end..]);

                stdin_field.text = Util.ForceCharacterWrap(sb.ToString());

                stdin_field.caretPosition = shell.prefixe._value.text.Length + last_reader.cpl_start + completion.Length;
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

                sb.Append(shell.prefixe._value.text);
                sb.Append(last_reader.text[..last_reader.cpl_start]);
                sb.Append(completion);
                sb.Append(last_reader.text[last_reader.cpl_end..]);

                stdin_field.text = Util.ForceCharacterWrap(sb.ToString());

                stdin_field.caretPosition = shell.prefixe._value.text.Length + last_reader.cpl_start + completion.Length;
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

            sb.Append(shell.prefixe._value.text);
            sb.Append(last_reader.text[..last_reader.cpl_start]);
            sb.Append(completion);
            sb.Append(last_reader.text[last_reader.cpl_end..]);

            stdin_field.text = Util.ForceCharacterWrap(sb.ToString());

            stdin_field.caretPosition = shell.prefixe._value.text.Length + last_reader.cpl_start + completion.Length;
        }
    }
}