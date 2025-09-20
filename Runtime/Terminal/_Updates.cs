﻿using _ARK_;
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

        public Command.Line.Linter linter = new();
        Command.Line.Linter ITerminal.GetLinter => linter;

        //--------------------------------------------------------------------------------------------------------------

        void OnUpdate()
        {
            if (IsWindowOpened)
            {
                if (inputs_hold.HasFlag(InputsFlags.Ctrl) && inputs_down.HasFlag(InputsFlags.L_key))
                    ClearStdout();

                if (mouse_scroll != 0)
                    if (new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition))
                        if (GetInputs_hold(InputsFlags.Ctrl))
                        {
                            Vector2 npos = scrollview.normalizedPosition;
                            font_size.Value = font_size.Value + mouse_scroll * 0.3f;
                            NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () => scrollview.normalizedPosition = npos;
                            NUCLEOR.delegates.Update_OnStartOfFrame_once += () => scrollview.normalizedPosition = npos;
                        }
                        else
                            scrollview.verticalNormalizedPosition = Mathf.Clamp01(scrollview.verticalNormalizedPosition + mouse_scroll * 0.1f);

                if (flag_alt.Value != default)
                    OnAltKey();

                if (flag_nav_history.TryPullValue(out KeyCode nav_val))
                {
                    Command.Line.OnHistoryNav(nav_val, out string entry);
                    input_stdin.input_field.text = entry;
                    input_stdin.input_field.caretPosition = entry.Length;
                    flag_stdin.Value = true;
                }
            }

            if (shell.current_state.status.state == CMD_STATES.BLOCKING)
                flag_progress.Value = true;

            if (shell.stdin_change_flag.PullValue())
            {
                flag_stdout.Value = true;
                if (shell.current_state.status.state == CMD_STATES.BLOCKING)
                    input_stdin.ResetText();
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnLateUpdate()
        {
            if (flag_progress.PullValue())
                RefreshProgressBars();

            if (flag_stdout.PullValue())
                RefreshStdout();

            if (flag_stdin.PullValue())
                RefreshStdin();

            if (flag_clampbottom.PullValue())
                ClampBottom();
        }

        void RefreshProgressBars()
        {
            if (shell.current_state.status.state == CMD_STATES.BLOCKING)
            {
                float body_width = rT_body.rect.width;
                float char_width = input_realtime.input_field.textComponent.GetPreferredValues("_", body_width, float.PositiveInfinity).x;
                int max_chars = (int)(body_width / char_width);

                float progress = shell.current_state.status.progress;

                int bar_count = max_chars - 5;
                int count = (int)(Mathf.Clamp01(progress) * bar_count);

                input_realtime.input_field.text = $"{new string('▓', count)}{new string('░', bar_count - count)} {Mathf.RoundToInt(100 * progress),3}%";

                flag_progress.Value = true;
            }
            else
                input_realtime.ResetText();
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