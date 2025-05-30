using _COBRA_;
using _SGUI_;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    partial class Constrictor
    {


        //--------------------------------------------------------------------------------------------------------------

        protected override void OnLint()
        {
            base.OnLint();

            SguiCompletor.instance.ResetIntellisense();

            string text = main_input_field.text;
            if (string.IsNullOrWhiteSpace(text))
                return;

            string[] lines = text.Split('\n');
            int character_count = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string text_line = lines[i];
                if (!string.IsNullOrWhiteSpace(text_line))
                {
                    lines[i] = terminal.linter.GetLint(terminal.shell, text_line, out Command.Line cmd_line, cursor_i: main_input_field.caretPosition - character_count, flags: SIG_FLAGS.LIST);
                    if (cmd_line.completions != null)
                    {
                        int text_index = character_count + cmd_line.last_start_i;
                        TMP_CharacterInfo info = main_input_field.textComponent.textInfo.characterInfo[text_index];

                        // Passe en world space
                        Vector3 worldPos = main_input_field.textComponent.rectTransform.TransformPoint(info.bottomLeft);

                        // Passe en écran (optionnel, utile pour placer une fenêtre UI dans le canvas Overlay)
                        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

                        SguiCompletor.instance.PopulateCompletions(character_count + cmd_line.last_start_i, character_count + cmd_line.last_read_i, screenPos, cmd_line.completions);
                    }
                }
                character_count += 1 + text_line.Length;
            }

            lint_tmp.text = lines.Join("\n");
        }
    }
}