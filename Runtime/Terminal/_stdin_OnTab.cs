﻿using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void OnTab(in int charIndex)
        {
            tab_frame = Time.frameCount;
            Command.Line line = new(
                stdin_save,
                SIG_FLAGS.TAB,
                shell,
                Mathf.Min(stdin_save.Length, charIndex),
                cpl_index++
                );

            shell.PropagateSignal(line);
            input_stdin.input_field.text = line.text;
            input_stdin.input_field.caretPosition = line.cursor_i;
        }
    }
}