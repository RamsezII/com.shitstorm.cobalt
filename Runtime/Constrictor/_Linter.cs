using _COBRA_;
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

            ResetIntellisense();

            string text = main_input_field.text;
            if (string.IsNullOrWhiteSpace(text))
                return;

            string[] lines = text.Split('\n');
            int character_count = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (!string.IsNullOrWhiteSpace(line))
                {
                    lines[i] = terminal.linter.GetLint(terminal.shell, line, out Command.Signal signal, cursor_i: main_input_field.caretPosition - character_count, flags: SIG_FLAGS.LIST);
                    if (signal.completions != null)
                    {
                        TMP_CharacterInfo info = main_input_field.textComponent.textInfo.characterInfo[signal.cpl_start_i];

                        // Passe en world space
                        Vector3 worldPos = main_input_field.textComponent.rectTransform.TransformPoint(info.bottomLeft);

                        // Passe en écran (optionnel, utile pour placer une fenêtre UI dans le canvas Overlay)
                        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

                        UpdateIntellisense(screenPos, signal.completions);
                    }
                }
                character_count += 1 + line.Length;
            }

            lint_tmp.text = lines.Join("\n");
        }
    }
}