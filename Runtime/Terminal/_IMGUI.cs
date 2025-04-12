using _ARK_;
using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        bool opened_once;

        //--------------------------------------------------------------------------------------------------------------

        bool OnOnGui(Event e)
        {
            if (e.type != EventType.KeyDown)
                return false;

            if (!sgui_toggle.Value)
            {
                bool toggle = false;

                if (!toggle)
                    if (e.keyCode == KeyCode.O && USAGES.AreEmpty(UsageGroups.Typing))
                        toggle = true;

                if (!toggle)
                    if ((e.control || e.command || e.alt) && e.keyCode == KeyCode.T)
                        toggle = true;

                if (toggle)
                {
                    base.sgui_toggle.Update(true);
                    if (!opened_once)
                    {
                        NUCLEOR.instance.subScheduler.AddRoutine(Util.EWaitForFrames(1, ClearStdout));
                        opened_once = true;
                    }
                    return true;
                }

                return false;
            }

            if (e.keyCode == KeyCode.Escape)
            {
                sgui_toggle.Update(false);
                return true;
            }

            if (input_stdin.input_field.isFocused)
            {
                if (e.alt)
                    switch (e.keyCode)
                    {
                        case KeyCode.LeftArrow:
                        case KeyCode.RightArrow:
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                            flag_alt.Update(e.keyCode);
                            return true;

#if UNITY_EDITOR
                        case KeyCode.S:
                            Shortcut_CtrlS();
                            return true;
#endif
                    }

                if (e.control || e.command)
                    switch (e.keyCode)
                    {
                        case KeyCode.S:
                            Shortcut_CtrlS();
                            return true;

                        case KeyCode.Backspace:
                            Shortcut_CtrlBackspace();
                            return true;
                    }

                switch (e.keyCode)
                {
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                        if (!e.alt && !e.control && !e.command)
                        {
                            flag_nav_history.Update(e.keyCode);
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        void Shortcut_CtrlS()
        {
            Command.Line line = new(string.Empty, SIGNALS.SAVE, this);
            shell.PropagateLine(line);
        }

        void Shortcut_CtrlBackspace()
        {
            switch (shell.current_status.state)
            {
                case CMD_STATES.BLOCKING:
                case CMD_STATES.FULLSCREEN_readonly:
                    {
                        if (!string.IsNullOrWhiteSpace(input_prefixe.input_field.text))
                            Debug.Log(input_prefixe.input_field.text, this);
                        Debug.Log("^C", this);

                        Command.Line line = new(string.Empty, SIGNALS.KILL, this);
                        shell.PropagateLine(line);

                        if (line.data.status == CMDLINE_STATUS.CONFIRM)
                        {
                            input_stdin.ResetText();
                            flag_stdin.Update(true);
                            hide_stdout.Update(false);
                            Debug.Log($"{shell} {line.signal} signal confirmed. {line.data}".ToSubLog());
                        }
                        else
                            Debug.LogWarning($"{shell} {line.signal} signal not confirmed. {line.data}");
                    }
                    break;

                case CMD_STATES.WAIT_FOR_STDIN:
                case CMD_STATES.FULLSCREEN_write:
                    if (input_stdin.input_field.caretPosition > 0)
                        if (!string.IsNullOrEmpty(input_stdin.input_field.text))
                        {
                            string text = input_stdin.input_field.text;
                            int caret = input_stdin.input_field.caretPosition;
                            int read_i = caret;

                            if (text.GroupedErase(ref read_i) > 0)
                            {
                                if (read_i > 0)
                                {
                                    input_stdin.input_field.text = text[..read_i] + text[caret..];
                                    input_stdin.input_field.caretPosition = read_i;
                                }
                                else
                                {
                                    input_stdin.ResetText();
                                    input_stdin.input_field.caretPosition = 0;
                                }

                                flag_stdin.Update(true);
                            }
                        }
                    break;

                default:
                    Debug.LogWarning($"CTRL+Backspace not supported in {shell.current_status.state} state.");
                    return;
            }
        }
    }
}