using _SGUI_;
using _UTIL_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        public readonly OnValue<KeyCode> flag_alt = new();
        [SerializeField] string stdin_save;
        [SerializeField] int cpl_index;
        [SerializeField] int stdin_frame, tab_frame;

        //--------------------------------------------------------------------------------------------------------------

        void IMGUI_global.IUser.OnOnGUI()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.alt)
                switch (e.keyCode)
                {
                    case KeyCode.LeftArrow:
                    case KeyCode.RightArrow:
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                        e.Use();
                        flag_alt.Update(e.keyCode);
                        break;
                }
        }

        void OnAltKey()
        {
            CMD_SIGNAL signal = flag_alt.PullValue switch
            {
                KeyCode.LeftArrow => CMD_SIGNAL.ALT_LEFT,
                KeyCode.RightArrow => CMD_SIGNAL.ALT_RIGHT,
                KeyCode.UpArrow => CMD_SIGNAL.ALT_UP,
                KeyCode.DownArrow => CMD_SIGNAL.ALT_DOWN,
                _ => 0,
            };

            if (signal == 0)
                return;

            tab_frame = Time.frameCount;
            stdin_save = input_stdin.input_field.text;

            try
            {
                CommandLine line = new(
                    stdin_frame >= tab_frame ? input_stdin.input_field.text : stdin_save,
                    signal,
                    input_stdin.input_field.caretPosition
                    );

                executors_stack._list[^1].Executate(line);
                stdin_save = line.text;
                input_stdin.input_field.text = line.text;
                input_stdin.input_field.caretPosition = line.cursor_i;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        void OnChangeStdin(string text)
        {
            if (tab_frame == Time.frameCount)
                return;

            cpl_index = 0;
            stdin_save = text;
            stdin_frame = Time.frameCount;
        }

        char OnValidateStdin(string text, int charIndex, char addedChar)
        {
            switch (addedChar)
            {
                case '\t':
                    tab_frame = Time.frameCount;
                    try
                    {
                        CommandLine line = new(
                            stdin_save,
                            CMD_SIGNAL.TAB,
                            Mathf.Min(stdin_save.Length, charIndex),
                            cpl_index++
                            );

                        executors_stack._list[^1].Executate(line);
                        input_stdin.input_field.text = line.text;
                        input_stdin.input_field.caretPosition = line.cursor_i;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);
                    }
                    return '\0';

                case '\n':
                    cpl_index = 0;
                    stdin_save = null;
                    Debug.Log(input_prefixe.input_field.text + " " + input_stdin.input_field.text);
                    try
                    {
                        executors_stack._list[^1].Executate(new CommandLine(input_stdin.input_field.text, CMD_SIGNAL.EXEC));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);
                    }
                    finally
                    {
                        input_stdin.input_field.text = null;
                    }
                    flag_stdin.Update(true);
                    return '\0';
            }
            return addedChar;
        }

        public void RefreshStdin()
        {
            CMD_STATUS status = executors_stack._list[^1].status;

            input_prefixe.input_field.text = status.prefixe;

            Vector2 prefered_dims = input_prefixe.input_field.textComponent.GetPreferredValues(status.prefixe + "_", scrollview.content.rect.width, float.PositiveInfinity);
            line_height = prefered_dims.y;

            input_stdin.rT.sizeDelta = new(-prefered_dims.x, 0);

            if (status.state == CMD_STATE.WAIT_FOR_STDIN)
            {
                float stdin_height = Mathf.Max(input_stdin.text_height, scrollview.viewport.rect.height);

                input_prefixe.AutoSize(false);
                input_stdin.AutoSize(false);

                rT_stdin.sizeDelta = new(rT_stdin.sizeDelta.x, stdin_height);

                scrollview.content.sizeDelta = new(0, 1 + input_stdout.text_height + input_realtime.text_height + stdin_height);
            }
            else
            {
                input_stdin.input_field.text = string.Empty;

                input_prefixe.AutoSize(false);
                input_stdin.AutoSize(false);

                rT_stdin.sizeDelta = new(rT_stdin.sizeDelta.x, scrollview.viewport.rect.height);

                scrollview.content.sizeDelta = new(0, 1 + input_stdout.text_height + input_realtime.text_height + scrollview.viewport.rect.height - line_height);
            }
        }

        public void ClampBottom()
        {
            float bottom_view = -scrollview.viewport.rect.height - scrollview.content.anchoredPosition.y;
            float bottom_stdin = -input_stdout.text_height - input_stdin.text_height;

            if (bottom_stdin < bottom_view)
                scrollview.verticalNormalizedPosition = Mathf.InverseLerp(-scrollview.content.rect.height, -scrollview.viewport.rect.height, bottom_stdin - 2 * line_height);
        }
    }
}