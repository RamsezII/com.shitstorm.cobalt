using _COBRA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void OnAltKey()
        {
            SIGNALS signal = flag_alt.PullValue switch
            {
                KeyCode.LeftArrow => SIGNALS.LEFT,
                KeyCode.RightArrow => SIGNALS.RIGHT,
                KeyCode.UpArrow => SIGNALS.UP,
                KeyCode.DownArrow => SIGNALS.DOWN,
                _ => 0,
            };

            if (signal == 0)
                return;

            signal |= SIGNALS.CPL_ALT;

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