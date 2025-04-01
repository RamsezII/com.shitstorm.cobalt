using _ARK_;
using _UTIL_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        public readonly OnValue<bool>
            flag_realtime = new(),
            flag_stdout = new(),
            flag_stdin = new(),
            flag_clampbottom = new();

        //--------------------------------------------------------------------------------------------------------------

        void OnUpdate()
        {
            if (inputs_hold.HasFlag(InputsFlags.Ctrl) && inputs_down.HasFlag(InputsFlags.L_key))
                ClearStdout();

            if (scroll_y != 0)
                if (new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition))
                    scrollview.verticalNormalizedPosition = Mathf.Clamp01(scrollview.verticalNormalizedPosition + scroll_y * 0.1f);
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnLateUpdate()
        {
            Command.Executor executor = executors_stack._list[^1];
            switch (executor.status.state)
            {
                case CMD_STATE.DONE:
                    executor.Dispose();
                    break;

                case CMD_STATE.BLOCKING:
                    flag_realtime.Update(true);
                    executor.Iterate();
                    break;

                case CMD_STATE.WAIT_FOR_STDIN:
                    break;
            }

            if (flag_realtime.PullValue)
            {
                if (executors_stack._list.Count == 1)
                    input_realtime.input_field.text = null;
                else
                {
                    CMD_STATUS status = executor.status;

                    float body_width = rT_body.rect.width;
                    float char_width = input_realtime.input_field.textComponent.GetPreferredValues("_", body_width, float.PositiveInfinity).x;
                    int max_chars = (int)(body_width / char_width);

                    int bar_count = max_chars - 5;
                    int count = (int)(Mathf.Clamp01(status.progress) * bar_count);

                    input_realtime.input_field.text = $"{new string('▓', count)}{new string('░', bar_count - count)} {Mathf.RoundToInt(100 * status.progress),3}%";
                }
                input_realtime.AutoSize(true);
                rT_scrollview.sizeDelta = new Vector2(0, -input_realtime.text_height);
            }

            if (flag_stdout.PullValue)
                RefreshStdout();

            if (flag_alt.Value != default)
                OnAltKey();

            if (flag_stdin.PullValue)
                RefreshStdin();

            if (flag_clampbottom.PullValue)
                NUCLEOR.delegates.onEndOfFrame_once += ClampBottom;
        }
    }
}