using UnityEngine;

namespace _COBALT_
{
    partial class InputText
    {
        char OnValidateStdin(string text, int charIndex, char addedChar)
        {
            terminal.flag_stdin.Update(true);
            switch (addedChar)
            {
                case '\t':
                    Debug.Log("COMPLETION");
                    return '\0';

                case '\n':
                    Debug.Log(input_field.text + " " + input_field.text);
                    input_field.text = null;
                    return '\0';
            }
            return addedChar;
        }
    }
}