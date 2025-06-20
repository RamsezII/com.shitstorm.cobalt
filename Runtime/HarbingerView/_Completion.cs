using _BOA_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        void OnTab(in string text, in int charIndex)
        {
            var reader = BoaReader.ReadLines(shell.lint_theme, false, charIndex, text);
            var signal = new BoaSignal(SIG_FLAGS_new.TAB, reader);

            shell.PropagateSignal(signal);

            string lint = reader.GetLintResult(Color.gray7);
        }
    }
}