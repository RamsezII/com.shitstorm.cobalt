using _COBRA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void OnAltKey()
        {
            SIG_FLAGS flags = flag_alt.PullValue() switch
            {
                KeyCode.LeftArrow => SIG_FLAGS.LEFT,
                KeyCode.RightArrow => SIG_FLAGS.RIGHT,
                KeyCode.UpArrow => SIG_FLAGS.UP,
                KeyCode.DownArrow => SIG_FLAGS.DOWN,
                _ => 0,
            };

            if (flags == 0)
                return;

            flags |= SIG_FLAGS.ALT;

            flag_stdin.Value = true;

            tab_frame = Time.frameCount;
            stdin_save = input_stdin.input_field.text;

            try
            {
                Command.Line line = new(
                    stdin_frame >= tab_frame ? input_stdin.input_field.text : stdin_save,
                    flags,
                    shell,
                    input_stdin.input_field.caretPosition
                    );

                shell.PropagateSignal(line);
                stdin_save = line.text;
                input_stdin.input_field.text = line.text;
                input_stdin.input_field.caretPosition = line.cursor_i;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}