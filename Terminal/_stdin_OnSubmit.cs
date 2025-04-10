using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void OnSubmit()
        {
            cpl_index = 0;
            stdin_save = null;

            if (shell.CurrentStatus.state == CMD_STATES.WAIT_FOR_STDIN)
            {
                string lint_text = linter.GetLint(this, input_stdin.input_field.text, out _);
                Debug.Log(input_prefixe.input_field.text + " " + lint_text, this);
            }

            Command.Line line = new(input_stdin.input_field.text, SIGNAL_FLAGS.CHECK, this);
            string error = shell.PropagateLine(line);

            if (error == null)
            {
                line = new(input_stdin.input_field.text, SIGNAL_FLAGS.EXEC, this);
                bool was_idle = shell.IsIdle;

                error = shell.PropagateLine(line);

                if (was_idle && error == null)
                    Command.Line.AddToHistory(line.text);

                hide_stdout.Update(shell.CurrentStatus.state switch
                {
                    CMD_STATES.FULLSCREEN_write or CMD_STATES.FULLSCREEN_readonly => true,
                    _ => false,
                });
            }
            input_stdin.ResetText();
        }
    }
}