using _SGUI_;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    partial class Terminal
    {
        private static void InitSoftwareButton()
        {
            SguiGlobal.instance.button_terminal.software_type = typeof(Terminal);
            SguiGlobal.instance.button_terminal.onClickAction = eventData =>
            {
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        if (SguiGlobal.instance.button_terminal.instances.IsEmpty)
                            InstantiateWindow<Terminal>(true, true, true);
                        break;

                    case PointerEventData.InputButton.Middle:
                        break;

                    case PointerEventData.InputButton.Right:
                        SguiGlobal.instance.button_terminal.dropdown.Show();
                        break;
                }
            };
        }
    }
}