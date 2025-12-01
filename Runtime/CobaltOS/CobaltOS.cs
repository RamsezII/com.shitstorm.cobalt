using _ARK_;
using _SGUI_;
using _ZOA_;
using UnityEngine;

namespace _COBALT_
{
    public static class CobaltOS
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            IMGUI_global.instance.users_inputs.AddElement(OnOnGuiInputs_static, new());
        }

        //--------------------------------------------------------------------------------------------------------------

        static bool OnOnGuiInputs_static(Event e)
        {
            if (e.type != EventType.KeyDown)
                return false;

            switch (e.keyCode)
            {
                case KeyCode.O:
                    if (UsageManager.AllAreEmpty(UsageGroups.Typing))
                        if (Terminal.instance_last == null)
                        {
                            OSView.instance.GetSoftwareButton<Terminal>(force: true).InstantiateSoftware();
                            return true;
                        }
                        else
                        {
                            Terminal.instance_last.ToggleWindow(true);
                            return true;
                        }
                    break;
            }

            if (e.control || e.command || e.alt)
                switch (e.keyCode)
                {
                    case KeyCode.T:
                    case KeyCode.O:
                        OSView.instance.GetSoftwareButton<Terminal>(force: true).InstantiateSoftware();
                        return true;

                    case KeyCode.I:
                        OSView.instance.GetSoftwareButton<SguiTerminal>(force: true).InstantiateSoftware();
                        return true;

                    case KeyCode.U:
                        OSView.instance.GetSoftwareButton<ZoaTerminal>(force: true).InstantiateSoftware();
                        return true;
                }

            return false;
        }
    }
}