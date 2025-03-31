using _SGUI_;
using _UTIL_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        public readonly OnValue<KeyCode> flag_alt = new();

        //--------------------------------------------------------------------------------------------------------------

        char OnValidateStdin(string text, int charIndex, char addedChar)
        {
            flag_stdin.Update(true);
            switch (addedChar)
            {
                case '\t':
                    Debug.Log("COMPLETION");
                    return '\0';

                case '\n':
                    Debug.Log(input_stdin.input_field.text + " " + input_stdin.input_field.text);
                    input_stdin.input_field.text = null;
                    return '\0';
            }
            return addedChar;
        }

        void IMGUI_global.IUser.OnOnGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown && e.alt)
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
        }

        void OnAltKey()
        {
            KeyCode key = flag_alt.PullValue;
        }
    }
}