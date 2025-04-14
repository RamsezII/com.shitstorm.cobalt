using _COBRA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void RefreshStdin()
        {
            input_prefixe.input_field.text = shell.current_status.prefixe;

            switch (shell.current_status.state)
            {
                case CMD_STATES.BLOCKING:
                    if (!string.IsNullOrEmpty(input_stdin.input_field.text))
                    {
                        input_stdin.ResetText();
                        stdin_save = string.Empty;
                    }
                    break;

                case CMD_STATES.FULLSCREEN_readonly:
                case CMD_STATES.FULLSCREEN_write:
                    if (!input_stdin.input_field.text.Equals(stdin_save, StringComparison.Ordinal))
                        input_stdin.input_field.text = stdin_save;
                    break;
            }

            Vector2 prefered_dims = input_prefixe.input_field.textComponent.GetPreferredValues(input_prefixe.input_field.text + "_", scrollview.content.rect.width, float.PositiveInfinity);
            line_height = prefered_dims.y;

            if (string.IsNullOrWhiteSpace(input_prefixe.input_field.text))
                prefered_dims.x = 0;

            input_stdin.rT.sizeDelta = new(-prefered_dims.x, 0);

            input_prefixe.AutoSize(false);
            input_stdin.AutoSize(false);

            linter_tmp.text = linter.GetLint(this, input_stdin.input_field.text, out _);

            const int stdin_offset = -2;

            if (string.IsNullOrWhiteSpace(input_stdin.input_field.text))
            {
                rT_stdin.sizeDelta = new(rT_stdin.sizeDelta.x, scrollview.viewport.rect.height);
                scrollview.content.sizeDelta = new(0, stdin_offset + input_stdout.text_height + input_realtime.text_height + scrollview.viewport.rect.height);
            }
            else
            {
                float stdin_height = Mathf.Max(input_stdin.text_height, scrollview.viewport.rect.height);

                rT_stdin.sizeDelta = new(rT_stdin.sizeDelta.x, stdin_height);
                scrollview.content.sizeDelta = new(0, stdin_offset + input_stdout.text_height + input_realtime.text_height + stdin_height);
            }

            flag_clampbottom.Update(true);
        }
    }
}