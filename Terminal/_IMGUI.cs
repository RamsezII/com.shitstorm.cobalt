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

            if (!isActive.Value)
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
                    isActive.Update(true);
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
                isActive.Update(false);
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
                    if (OnCtrl_keycode(e.keyCode))
                        return true;

                if (executor.routine == null)
                    if (!e.alt && !e.control && !e.command)
                        switch (e.keyCode)
                        {
                            case KeyCode.UpArrow:
                            case KeyCode.DownArrow:
                                flag_nav_history.Update(e.keyCode);
                                e.Use();
                                return true;
                        }
            }

            return false;
        }

        bool OnCtrl_keycode(in KeyCode ctrl_val)
        {
            switch (ctrl_val)
            {
                case KeyCode.C:
                    Debug.Log("^C", this);
                    if (executor.TryKill())
                    {
                        input_stdin.ResetText();
                        flag_stdin.Update(true);
                        hide_stdout.Update(false);
                    }
                    return true;

                case KeyCode.S:
                    executor.Executate(new(string.Empty, CMD_SIGNALS.SAVE, this, linter));
                    return true;

                case KeyCode.Backspace:
                    if (input_stdin.input_field.caretPosition > 0)
                        if (!string.IsNullOrEmpty(input_stdin.input_field.text))
                        {
                            string text = input_stdin.input_field.text;
                            int caret = input_stdin.input_field.caretPosition;
                            int erase_i = caret;

                            if (text.EndsWith(Util_cobra.CHAR_NEWLINE))
                            {
                                if (Util_cobra.SkipCharactersUntil(text, ref erase_i, false, false, Util_cobra.CHAR_NEWLINE) > 1)
                                    if (erase_i > 0)
                                        ++erase_i;
                            }
                            else
                            {
                                Util_cobra.SkipCharactersUntil(text, ref erase_i, false, false, Util_cobra.CHAR_SPACE);
                                Util_cobra.SkipCharactersUntil(text, ref erase_i, false, true, Util_cobra.CHAR_SPACE);
                            }

                            if (erase_i < caret)
                            {
                                if (erase_i > 0)
                                {
                                    input_stdin.input_field.text = text[..erase_i] + text[caret..];
                                    input_stdin.input_field.caretPosition = erase_i;
                                }
                                else
                                {
                                    input_stdin.ResetText();
                                    input_stdin.input_field.caretPosition = 0;
                                }

                                flag_stdin.Update(true);
                            }
                        }
                    return true;

                default:
                    return false;
            }
        }
    }
}