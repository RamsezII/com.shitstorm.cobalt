using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void OnTab(in int charIndex)
        {
            tab_frame = Time.frameCount;
            Command.Signal signal = new(
                stdin_save,
                SIG_FLAGS.TAB,
                shell,
                Mathf.Min(stdin_save.Length, charIndex),
                cpl_index++
                );

            shell.PropagateSignal(signal);
            input_stdin.input_field.text = signal.text;
            input_stdin.input_field.caretPosition = signal.cursor_i;
        }
    }
}