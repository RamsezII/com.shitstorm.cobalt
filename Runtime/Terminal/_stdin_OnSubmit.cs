using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void OnSubmit()
        {
            cpl_index = 0;
            stdin_save = string.Empty;
            string input_text = input_stdin.input_field.text;
            input_stdin.ResetText();

            if (shell.current_status.state == CMD_STATES.WAIT_FOR_STDIN)
            {
                if (string.IsNullOrWhiteSpace(input_text))
                {
                    sgui_toggle_window.Update(false);
                    return;
                }

                string lint_text = linter.GetLint(this, input_text, out _);
                Debug.Log(input_prefixe.input_field.text + " " + lint_text, this);
            }

            Command.Line line = new(input_text, SIGNALS.CHECK, this);
            string error = shell.PropagateLine(line);

            if (error == null)
            {
                line = new(input_text, SIGNALS.EXEC, this);
                bool was_idle = shell.IsIdle;

                error = shell.PropagateLine(line);

                if (was_idle && error == null)
                    Command.Line.AddToHistory(line.text);
            }
        }
    }
}