using _ARK_;
using _UTIL_;
using System.Collections.Generic;
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
            if (inputs_hold.HasFlag(InputsFlags.Ctrl) && inputs_down.HasFlag(InputsFlags.L_key))
                ClearStdout();

            if (scroll_y != 0)
                if (new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition))
                    scrollview.verticalNormalizedPosition = Mathf.Clamp01(scrollview.verticalNormalizedPosition + scroll_y * 0.1f);

            Command.Executor executor = executors_stack._list[^1];
            switch (executor.status.state)
            {
                case CMD_STATE.DONE:
                    executor.Dispose();
                    break;

                case CMD_STATE.BLOCKING:
                    executor.Iterate();
                    break;

                case CMD_STATE.WAIT_FOR_STDIN:
                    break;
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnLateUpdate()
        {
            if (flag_progress.PullValue)
                RefreshProgressBars();

            if (flag_stdout.PullValue)
                RefreshStdout();

            if (flag_alt.Value != default)
                OnAltKey();

            if (flag_stdin.PullValue)
                RefreshStdin();

            if (flag_clampbottom.PullValue)
                NUCLEOR.delegates.onEndOfFrame_once += ClampBottom;
        }

        void OnNucleorBusiness(List<Schedulable> list) => flag_progress.Update(true);
        void RefreshProgressBars()
        {
            if (NUCLEOR.instance.scheduler.list.IsEmpty || NUCLEOR.instance.scheduler.list._list[0].routine == null)
                input_realtime.input_field.text = null;
            else
            {
                float body_width = rT_body.rect.width;
                float char_width = input_realtime.input_field.textComponent.GetPreferredValues("_", body_width, float.PositiveInfinity).x;
                int max_chars = (int)(body_width / char_width);

                float progress = NUCLEOR.instance.scheduler.list._list[0].routine.Current;

                int bar_count = max_chars - 5;
                int count = (int)(Mathf.Clamp01(progress) * bar_count);

                input_realtime.input_field.text = $"{new string('▓', count)}{new string('░', bar_count - count)} {Mathf.RoundToInt(100 * progress),3}%";

                flag_progress.Update(true);
            }
            input_realtime.AutoSize(true);
            rT_scrollview.sizeDelta = new Vector2(0, -input_realtime.text_height);
        }
    }
}