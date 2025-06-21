using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        [SerializeField] BoaReader last_reader;
        [SerializeField] string stdin_save;
        [SerializeField] int last_tab;

        //----------------------------------------------------------------------------------------------------------

        void OnChange()
        {
            stdin_field.lint.text = shell.current_status.prefixe_lint;

            if (GetStdin(out string text, out int cursor_i))
            {
                last_reader = BoaReader.ReadLines(shell.lint_theme, false, cursor_i, text);

                var signal = new BoaSignal(SIG_FLAGS_new.CHANGE, last_reader);
                shell.PropagateSignal(signal);

                stdin_field.lint.text += last_reader.GetLintResult(Color.gray6);
            }

            if (Time.frameCount > last_tab)
                stdin_save = text;
        }

        void OnTab()
        {
            if (last_reader != null && last_reader.completions != null)
            {
                string text = $"{last_reader.completions.Count} completions: {last_reader.completions.Join(" ")}";
                shell.AddLine(text);
                Debug.Log(text, this);
            }
        }
    }
}