using _SGUI_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal : IMGUI_global.IUser
    {
        void IMGUI_global.IUser.OnOnGUI()
        {
            Event e = Event.current;
            if (e.type != EventType.KeyDown)
                return;

            if (e.keyCode == KeyCode.Escape)
            {
                flag_escape.Update(true);
                e.Use();
            }

            if (e.alt)
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

            if (e.control || e.command)
                switch (e.keyCode)
                {
                    case KeyCode.Backspace:
                        e.Use();
                        flag_ctrl.Update(e.keyCode);
                        break;
                }

            if (!e.alt && !e.control && !e.command)
                switch (e.keyCode)
                {
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                        e.Use();
                        flag_nav_history.Update(e.keyCode);
                        break;
                }
        }
    }
}