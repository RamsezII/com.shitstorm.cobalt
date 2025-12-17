using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        bool OnImguiInputs(Event e)
        {
            if (!stdin_field.isFocused)
                return false;

            if (e.isKey)
                if (e.type == EventType.KeyDown)
                {
                    if (e.control || e.command)
                    {
                        switch (e.keyCode)
                        {
                            case KeyCode.A:
                                NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () =>
                                {
                                    stdin_field.caretPosition = stdin_field.text.Length;
                                    stdin_field.selectionAnchorPosition = shell.status._value.prefixe.Text.Length;
                                };
                                return true;
                        }
                        return false;
                    }

                    if (e.alt)
                    {
                        switch (e.keyCode)
                        {
                            case KeyCode.UpArrow:
                            case KeyCode.DownArrow:
                                OnAlt_up_down(e.keyCode);
                                return true;

                            case KeyCode.LeftArrow:
                            case KeyCode.RightArrow:
                                OnAlt_left_right(e.keyCode);
                                return true;
                        }
                        return false;
                    }

                    switch (e.keyCode)
                    {
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                            {
                                int nav = e.keyCode switch
                                {
                                    KeyCode.UpArrow => -1,
                                    KeyCode.DownArrow => 1,
                                    _ => throw new System.Exception("impossible"),
                                };

                                if (nav != 0)
                                    if (TryNavHistory(nav, out string value))
                                    {
                                        flag_history = true;
                                        stdin_field.text = Util.ForceCharacterWrap(shell.status._value.prefixe.Text + value);
                                        stdin_field.caretPosition = stdin_field.text.Length;
                                    }

                                return true;
                            }
                    }
                }

            return false;
        }
    }
}