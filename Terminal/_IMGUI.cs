using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        bool opened_once;

        //--------------------------------------------------------------------------------------------------------------

        bool OnOnGui_toggle(Event e)
        {
            lock (isActive)
                if (!isActive._value)
                {
                    bool toggle = false;

                    if (!toggle)
                        if (e.keyCode == KeyCode.P && USAGES.AreEmpty(UsageGroups.Typing))
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
                }
            return false;
        }

        bool OnOnGui_shortcuts(Event e)
        {
            if (e.keyCode == KeyCode.Escape)
            {
                flag_escape.Update(true);
                return true;
            }

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
                        flag_ctrl.Update(e.keyCode);
                        return true;
                }

            if (!e.alt && !e.control && !e.command)
                switch (e.keyCode)
                {
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                        flag_nav_history.Update(e.keyCode);
                        e.Use();
                        return true;
                }

            return false;
        }
    }
}