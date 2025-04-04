using _ARK_;
using _UTIL_;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        public readonly OnValue<bool>
            flag_progress = new(),
            flag_stdout = new(),
            flag_stdin = new(),
            flag_clampbottom = new();

        //--------------------------------------------------------------------------------------------------------------

        void OnUpdate()
        {
            if (isActive._value)
            {
                if (inputs_hold.HasFlag(InputsFlags.Ctrl) && inputs_down.HasFlag(InputsFlags.L_key))
                    ClearStdout();

                if (scroll_y != 0)
                    if (new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition))
                        scrollview.verticalNormalizedPosition = Mathf.Clamp01(scrollview.verticalNormalizedPosition + scroll_y * 0.1f);
            }

            if (executor.routine != null)
            {
                flag_progress.Update(true);
                if (executor.routine.Current.state == CMD_STATES.BLOCKING)
                {
                    executor.Executate(Command.Line.EMPTY_EXE);
                    if (executor.routine == null)
                        flag_stdin.Update(true);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnLateUpdate()
        {
            if (flag_progress.PullValue)
                RefreshProgressBars();

            if (flag_stdout.PullValue)
                RefreshStdout();

            if (flag_ctrl.TryPullValue(out KeyCode ctrl_val))
                if (!string.IsNullOrEmpty(input_stdin.input_field.text))
                {
                    string text = input_stdin.input_field.text;
                    int caret = input_stdin.input_field.caretPosition;
                    int erase_i = caret;

                    Util_ark.SkipCharactersUntil(text, ref erase_i, false, false, Util_ark.CHAR_SPACE);
                    Util_ark.SkipCharactersUntil(text, ref erase_i, false, true, Util_ark.CHAR_SPACE);

                    if (erase_i < caret)
                    {
                        input_stdin.input_field.text = text[..erase_i] + text[caret..];
                        input_stdin.input_field.caretPosition = erase_i;
                        flag_stdin.Update(true);
                    }
                }

            if (flag_alt.Value != default)
                OnAltKey();

            if (flag_nav_history.TryPullValue(out KeyCode nav_val))
                executor.OnHistoryNav(nav_val);

            if (flag_stdin.PullValue)
                RefreshStdin();

            if (flag_clampbottom.PullValue)
                NUCLEOR.delegates.onEndOfFrame_once += ClampBottom;
        }

        void RefreshProgressBars()
        {
            if (executor.routine == null || executor.routine.Current.state != CMD_STATES.BLOCKING)
                input_realtime.input_field.text = string.Empty;
            else
            {
                float body_width = rT_body.rect.width;
                float char_width = input_realtime.input_field.textComponent.GetPreferredValues("_", body_width, float.PositiveInfinity).x;
                int max_chars = (int)(body_width / char_width);

                float progress = executor.routine.Current.progress;

                int bar_count = max_chars - 5;
                int count = (int)(Mathf.Clamp01(progress) * bar_count);

                input_realtime.input_field.text = $"{new string('▓', count)}{new string('░', bar_count - count)} {Mathf.RoundToInt(100 * progress),3}%";
            }
            input_realtime.AutoSize(true);
            rT_scrollview.sizeDelta = new Vector2(0, -input_realtime.text_height);
        }

        public void RefreshStdout()
        {
            StringBuilder sb = new();
            lock (lines)
            {
                foreach (object line in lines)
                    sb.AppendLine(line.ToString());
            }
            stdout = sb.TroncatedForLog();

            input_stdout.input_field.text = stdout;
            input_stdout.AutoSize(true);

            RefreshStdin();
            flag_clampbottom.Update(true);
        }

        public void RefreshStdin()
        {
            bool no_stdin = false;
            if (executor.routine == null)
                input_prefixe.input_field.text = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{executor.cmd_path.SetColor("#73B2D9")}$";
            else if (executor.routine.Current.state == CMD_STATES.WAIT_FOR_STDIN)
                input_prefixe.input_field.text = executor.routine.Current.prefixe;
            else
            {
                no_stdin = true;
                input_prefixe.input_field.text = string.Empty;
            }

            Vector2 prefered_dims = input_prefixe.input_field.textComponent.GetPreferredValues(input_prefixe.input_field.text + "_", scrollview.content.rect.width, float.PositiveInfinity);
            line_height = prefered_dims.y;

            if (string.IsNullOrWhiteSpace(input_prefixe.input_field.text))
                prefered_dims.x = 0;

            input_stdin.rT.sizeDelta = new(-prefered_dims.x, 0);

            input_prefixe.AutoSize(false);
            input_stdin.AutoSize(false);

            if (no_stdin)
            {
                rT_stdin.sizeDelta = new(rT_stdin.sizeDelta.x, scrollview.viewport.rect.height);
                scrollview.content.sizeDelta = new(0, 1 + input_stdout.text_height + input_realtime.text_height + scrollview.viewport.rect.height - line_height);
            }
            else
            {
                float stdin_height = Mathf.Max(input_stdin.text_height, scrollview.viewport.rect.height);

                rT_stdin.sizeDelta = new(rT_stdin.sizeDelta.x, stdin_height);
                scrollview.content.sizeDelta = new(0, 1 + input_stdout.text_height + input_realtime.text_height + stdin_height);
            }
        }
    }
}