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
            flag_stdin.Update(false);

            string prefixe = $"{MachineSettings.machine_name.Value.SetColor("#73CC26")}:{shell.prefixe.SetColor("#73B2D9")}$";
            input_prefixe.input_field.text = prefixe;

            float prefixe_width = input_prefixe.input_field.textComponent.GetPreferredValues(prefixe + "_", scroll_view.content.rect.width, float.PositiveInfinity).x;
            float stdin_height = Mathf.Max(input_stdin.text_height, scroll_view.viewport.rect.height);

            input_realtime.AutoSize(true);
            input_prefixe.AutoSize(true);
            input_stdin.AutoSize(false);

            input_stdin.rT.sizeDelta = new(-prefixe_width, 0);
            rT_stdin.sizeDelta = new(0, stdin_height);

            scroll_view.content.sizeDelta = new(0, input_stdout.text_height + input_realtime.text_height + stdin_height);
        }
    }
}