using _COBRA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void OnChangeStdin(string text)
        {
            if (tab_frame == Time.frameCount)
                return;

            cpl_index = 0;
            stdin_frame = Time.frameCount;
            flag_stdin.Update(true);

            if (!string.IsNullOrEmpty(text))
                switch (shell.current_status.state)
                {
                    case CMD_STATES.BLOCKING:
                        stdin_save = string.Empty;
                        if (!string.IsNullOrEmpty(input_stdin.input_field.text))
                            input_stdin.ResetText();
                        break;

                    case CMD_STATES.FULLSCREEN_readonly:
                        if (text.Equals(input_stdin.input_field.text, StringComparison.Ordinal))
                            input_stdin.input_field.text = stdin_save;
                        break;

                    case CMD_STATES.FULLSCREEN_write:
                    case CMD_STATES.WAIT_FOR_STDIN:
                    default:
                        stdin_save = text;
                        Command.Line line = new(text, SIGNALS.STDIN_CHANGE, this);
                        shell.PropagateLine(line);
                        break;
                }
        }
    }
}