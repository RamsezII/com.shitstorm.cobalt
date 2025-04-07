using _COBRA_;
using _UTIL_;
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

        [SerializeField] Command.Line.Linter linter = new();

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

                if (flag_ctrl.TryPullValue(out KeyCode ctrl_val))
                    OnCtrl_keycode(ctrl_val);

                if (flag_alt.Value != default)
                    OnAltKey();

                if (flag_nav_history.TryPullValue(out KeyCode nav_val))
                {
                    Command.Line.OnHistoryNav(nav_val, out string entry);
                    input_stdin.input_field.text = entry;
                    input_stdin.input_field.caretPosition = entry.Length;
                    flag_stdin.Update(true);
                }
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

            if (flag_stdin.PullValue)
                RefreshStdin();

            if (flag_clampbottom.PullValue)
                ClampBottom();
        }

        void RefreshProgressBars()
        {
            if (executor.routine == null || executor.routine.Current.state != CMD_STATES.BLOCKING)
                input_realtime.ResetText();
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

        void ClampBottom()
        {
            float new_height = Mathf.InverseLerp(
                -scrollview.content.rect.height,
                -scrollview.viewport.rect.height,
                -input_stdout.text_height - Mathf.Max(line_height, input_stdin.text_height) - 2 * line_height
                );

            if (new_height < scrollview.verticalNormalizedPosition)
                scrollview.verticalNormalizedPosition = new_height;
        }
    }
}