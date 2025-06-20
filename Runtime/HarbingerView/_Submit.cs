using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        void OnSubmit(in string text)
        {
            std_in.ResetText();

            var reader = BoaReader.ReadLines(shell.lint_theme, false, std_in.inputfield.caretPosition, text);
            var signal = new BoaSignal(SIG_FLAGS_new.SUBMIT, reader);

            shell.PropagateSignal(signal);

            string lint = reader.GetLintResult(Color.gray7);
            shell.AddLine(text, lint);

            if (reader.error != null)
                Debug.LogError(reader.error);
        }
    }
}