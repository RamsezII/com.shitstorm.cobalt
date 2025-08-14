using _ARK_;
using _COBRA_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        bool opened_once;

        //--------------------------------------------------------------------------------------------------------------

        bool OnOnGuiInputs(Event e)
        {
            if (e.type != EventType.KeyDown)
                return false;

            if (!IsWindowOpened)
            {
                bool toggle = false;

                if (!toggle)
                    if (e.keyCode == KeyCode.O && UsageManager.AreEmpty(UsageGroups.Typing))
                        toggle = true;

                if (toggle)
                {
                    ToggleWindow(true);
                    if (!opened_once)
                    {
                        NUCLEOR.instance.subSequencer.AddRoutine(Util.EWaitForFrames(1, ClearStdout));
                        opened_once = true;
                    }
                    return true;
                }

                return false;
            }

            if (e.keyCode == KeyCode.Escape)
            {
                ToggleWindow(false);
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
                    }

                if (e.control || e.command)
                    switch (e.keyCode)
                    {
                        case KeyCode.Backspace:
                            if (Shortcut_CtrlBackspace())
                                return true;
                            break;
                    }

                if (e.alt)
                    switch (e.keyCode)
                    {
                        case KeyCode.Backspace:
                            if (Shortcut_AltBackspace())
                                return true;
                            break;
                    }

                if (shell.current_state.status.state == CMD_STATES.WAIT_FOR_STDIN)
                    switch (e.keyCode)
                    {
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                            if (!e.alt && !e.control && !e.command)
                            {
                                Command.Line line = new(string.Empty, e.keyCode switch
                                {
                                    KeyCode.UpArrow => SIG_FLAGS.HIST_UP,
                                    KeyCode.DownArrow => SIG_FLAGS.HIST_DOWN,
                                    _ => SIG_FLAGS.HIST,
                                },
                                shell);

                                shell.PropagateSignal(line);

                                flag_nav_history.Update(e.keyCode);
                                return true;
                            }
                            break;
                    }
            }

            return false;
        }

        bool Shortcut_AltBackspace()
        {
            if (!string.IsNullOrWhiteSpace(input_prefixe.input_field.text))
                Debug.Log(input_prefixe.input_field.text, this);
            Debug.Log("^C", this);

            if (!shell.IsBusy)
                Debug.LogWarning($"{shell}: no operation to kill");
            else
            {
                Command.Line line = new(string.Empty, SIG_FLAGS.KILL, shell);
                shell.PropagateSignal(line);

                if (line.data.status == CMDLINE_STATUS.CONFIRM)
                {
                    input_stdin.ResetText();
                    flag_stdin.Update(true);
                    Debug.Log($"{shell} {line.flags} signal confirmed. {line.data}".ToSubLog());
                }
                else
                    Debug.LogWarning($"{shell} {line.flags} signal not confirmed. {line.data}");
            }

            return true;
        }

        bool Shortcut_CtrlBackspace()
        {
            if (input_stdin.input_field.caretPosition > 0)
                if (!string.IsNullOrEmpty(input_stdin.input_field.text))
                {
                    string text = input_stdin.input_field.text;
                    int caret = input_stdin.input_field.caretPosition;
                    int read_i = caret;

                    if (shell.current_state.status.state == CMD_STATES.WAIT_FOR_STDIN)
                    {
                        Command.Line line = new(input_stdin.input_field.text, SIG_FLAGS._none_, shell, input_stdin.input_field.caretPosition, cpl_index);
                        shell.PropagateSignal(line);

                        if (line.is_cursor_on_path)
                        {
                            int index = line.path_last.LastIndexOf('/');
                            if (index == -1)
                                index = line.path_i;
                            else
                            {
                                index += line.path_i;
                                if (!line.path_last.EndsWith('/'))
                                    ++index;
                            }

                            stdin_save = input_stdin.input_field.text = text[..index];
                            input_stdin.input_field.caretPosition = index;

                            flag_stdin.Update(true);
                            return true;
                        }
                    }

                    if (text.GroupedErase(ref read_i) > 0)
                    {
                        input_stdin.input_field.text = text[..read_i] + text[caret..];
                        input_stdin.input_field.caretPosition = read_i;
                    }

                    stdin_save = input_stdin.input_field.text;
                    flag_stdin.Update(true);
                }
            return true;
        }
    }
}