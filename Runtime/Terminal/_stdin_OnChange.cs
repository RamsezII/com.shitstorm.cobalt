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
            stdin_frame = Time.frameCount;
            flag_stdin.Update(true);

            switch (shell.current_status.state)
            {
                case CMD_STATES.WAIT_FOR_STDIN:
                    stdin_save = text;
                    break;

                case CMD_STATES.BLOCKING:
                default:
                    stdin_save = string.Empty;
                    if (!string.IsNullOrEmpty(input_stdin.input_field.text))
                        input_stdin.ResetText();
                    break;
            }
        }
    }
}