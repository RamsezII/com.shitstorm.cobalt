using _COBRA_;
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
            stdin_save = text;
            stdin_frame = Time.frameCount;
            flag_stdin.Update(true);

            if (!string.IsNullOrWhiteSpace(text))
                switch (shell.CurrentStatus.state)
                {
                    case CMD_STATES.BLOCKING:
                        input_stdin.ResetText();
                        break;

                    case CMD_STATES.FULLSCREEN_readonly:
                        break;

                    case CMD_STATES.FULLSCREEN_write:
                    case CMD_STATES.WAIT_FOR_STDIN:
                    default:
                        Command.Line line = new(text, SIGNALS.STDIN_CHANGE, this);
                        shell.PropagateLine(line);
                        break;
                }
        }
    }
}