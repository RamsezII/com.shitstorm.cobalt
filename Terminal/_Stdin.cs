using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        char OnValidateStdin(string text, int charIndex, char addedChar)
        {
            switch (addedChar)
            {
                case '\t':
                    Debug.Log("COMPLETION");
                    return '\0';

                case '\n':
                    {
                        Debug.Log(input_prefixe.input_field.text + " " + input_stdin.input_field.text);
                        input_stdin.input_field.text = null;
                        return '\0';
                    }
            }
            return addedChar;
        }

        public void RefreshStdin()
        {
            refresh_stdin.Update(false);

            string prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{shell.prefixe.SetColor("#73B2D9")}$";
            input_prefixe.input_field.text = prefixe;

            input_realtime.AutoSize();
            input_prefixe.AutoSize();
            input_stdin.AutoSize();

            scroll_view.content.sizeDelta = new(0, input_stdout.text_height + input_realtime.text_height + input_stdin.text_height);
        }
    }
}