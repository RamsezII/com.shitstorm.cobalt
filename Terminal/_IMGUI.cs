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

#if UNITY_EDITOR
                        case KeyCode.S:
                            Shortcut_CtrlS();
                            return true;
#endif
                    }

                if (e.control || e.command)
                    switch (e.keyCode)
                    {
                        case KeyCode.C:
                            Shortcut_CtrlC();
                            return true;

                        case KeyCode.S:
                            Shortcut_CtrlS();
                            return true;

                        case KeyCode.Backspace:
                            Shortcut_CtrlBackspace();
                            return true;
                    }

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

        void Shortcut_CtrlC()
        {
            if (executor.routine != null)
                if (!string.IsNullOrWhiteSpace(input_prefixe.input_field.text))
                    Debug.Log(input_prefixe.input_field.text, this);
            Debug.Log("^C", this);

            if (executor.TryKill())
            {
                input_stdin.ResetText();
                flag_stdin.Update(true);
                hide_stdout.Update(false);
            }
        }

        void Shortcut_CtrlS()
        {
            executor.Executate(new(string.Empty, CMD_SIGNALS.EXEC | CMD_SIGNALS.SAVE, this));
            flag_stdout.Update(true);
        }

        void Shortcut_CtrlBackspace()
        {
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
        }
    }
}