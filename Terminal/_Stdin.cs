using _COBRA_;
using _UTIL_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        public readonly OnValue<KeyCode>
            flag_alt = new(),
            flag_ctrl = new(),
            flag_nav_history = new();

        public readonly OnValue<bool>
            flag_escape = new();

        [SerializeField] string stdin_save;
        [SerializeField] int cpl_index;
        [SerializeField] int stdin_frame, tab_frame;

        //--------------------------------------------------------------------------------------------------------------

        void OnAltKey()
        {
            CMD_SIGNALS signal = flag_alt.PullValue switch
            {
                KeyCode.LeftArrow => CMD_SIGNALS.ALT_LEFT,
                KeyCode.RightArrow => CMD_SIGNALS.ALT_RIGHT,
                KeyCode.UpArrow => CMD_SIGNALS.ALT_UP,
                KeyCode.DownArrow => CMD_SIGNALS.ALT_DOWN,
                _ => 0,
            };

            if (signal == 0)
                return;

            flag_stdin.Update(true);

            tab_frame = Time.frameCount;
            stdin_save = input_stdin.input_field.text;

            try
            {
                Command.Line line = new(
                    stdin_frame >= tab_frame ? input_stdin.input_field.text : stdin_save,
                    signal,
                    linter,
                    input_stdin.input_field.caretPosition
                    );

                executor.Executate(line);
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
            flag_stdin.Update(true);

            if (executor.routine != null)
                switch (executor.routine.Current.state)
                {
                    case CMD_STATES.BLOCKING:
                    case CMD_STATES.FULLSCREEN_r:
                        input_stdin.ResetText();
                        break;
                }
        }

        char OnValidateStdin(string text, int charIndex, char addedChar)
        {
            flag_stdin.Update(true);
            Command.Line.ResetHistoryCount();
            switch (addedChar)
            {
                case '\t':
                    tab_frame = Time.frameCount;
                    try
                    {
                        Command.Line line = new(
                            stdin_save,
                            CMD_SIGNALS.TAB,
                            linter,
                            Mathf.Min(stdin_save.Length, charIndex),
                            cpl_index++
                            );

                        executor.Executate(line);
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
                    if (string.IsNullOrWhiteSpace(input_stdin.input_field.text))
                        isActive.Update(false);
                    else
                        try
                        {
                            string lint_text = linter.GetLint(executor, input_stdin.input_field.text);
                            Debug.Log(input_prefixe.input_field.text + " " + lint_text, this);

                            Command.Line line = new(input_stdin.input_field.text, CMD_SIGNALS.CHECK, linter);
                            executor.Executate(line);

                            if (executor.error == null)
                            {
                                line = new(input_stdin.input_field.text, CMD_SIGNALS.EXEC, linter);
                                bool noRoutine = executor.routine == null;

                                executor.Executate(line);

                                if (noRoutine && executor.error == null)
                                    Command.Line.AddToHistory(line.text);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e, this);
                        }
                    input_stdin.ResetText();
                    return '\0';
            }
            return addedChar;
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