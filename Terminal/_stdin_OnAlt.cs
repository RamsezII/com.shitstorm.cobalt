using _COBRA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void OnAltKey()
        {
            SIGNAL_FLAGS signal = flag_alt.PullValue switch
            {
                KeyCode.LeftArrow => SIGNAL_FLAGS.LEFT,
                KeyCode.RightArrow => SIGNAL_FLAGS.RIGHT,
                KeyCode.UpArrow => SIGNAL_FLAGS.UP,
                KeyCode.DownArrow => SIGNAL_FLAGS.DOWN,
                _ => 0,
            };

            if (signal == 0)
                return;

            signal |= SIGNAL_FLAGS.CPL_ALT;

            flag_stdin.Update(true);

            tab_frame = Time.frameCount;
            stdin_save = input_stdin.input_field.text;

            try
            {
                Command.Line line = new(
                    stdin_frame >= tab_frame ? input_stdin.input_field.text : stdin_save,
                    signal,
                    this,
                    input_stdin.input_field.caretPosition
                    );

                shell.PropagateLine(line);
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