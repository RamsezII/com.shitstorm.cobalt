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
            if (flag_realtime.PullValue)
                RefreshRealtime();

            if (flag_stdout.PullValue)
                RefreshStdout();

            if (flag_alt.Value != default)
                OnAltKey();

            if (flag_stdin.PullValue)
                RefreshStdin();

            if (flag_clampbottom.PullValue)
                NUCLEOR.delegates.onEndOfFrame_once += ClampBottom;
        }

        void RefreshRealtime()
        {
            input_realtime.AutoSize(true);
            rT_scrollview.sizeDelta = new Vector2(0, -input_realtime.text_height);
        }
    }
}