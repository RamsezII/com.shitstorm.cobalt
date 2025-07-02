using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    partial class HarbingerView
    {
        protected override bool OnImguiInputs(Event e)
        {
            if (e.isKey)
                if (e.type == EventType.KeyDown)
                {
                    if (e.control || e.command)
                    {
                        switch (e.keyCode)
                        {
                            case KeyCode.A:
                                NUCLEOR.delegates.onEndOfFrame_once += () =>
                                {
                                    stdin_field.caretPosition = stdin_field.text.Length;
                                    stdin_field.selectionAnchorPosition = shell.current_status.prefixe_text.Length;
                                };
                                return true;
                        }
                        return base.OnImguiInputs(e);
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
                        return base.OnImguiInputs(e);
                    }

                    switch (e.keyCode)
                    {
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                            if (!shell.IsBusy)
                            {
                                int nav = e.keyCode switch
                                {
                                    KeyCode.UpArrow => -1,
                                    KeyCode.DownArrow => 1,
                                    _ => 0,
                                };

                                if (nav != 0)
                                    if (shell.TryNavHistory(nav, out string value))
                                    {
                                        flag_history = true;
                                        stdin_field.text = shell.current_status.prefixe_text + value;
                                        stdin_field.caretPosition = stdin_field.text.Length;
                                    }

                                return true;
                            }
                            break;
                    }
                }

            return base.OnImguiInputs(e);
        }
    }
}