using _BOA_;
using _SGUI_;
using TMPro;
using UnityEngine;

namespace _COBALT_
{
    partial class Constrictor
    {
        [SerializeField] bool strict_syntax;
        [SerializeField] string script_path;
        [SerializeField] string workdir;

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnLint()
        {
            base.OnLint();

            if (settings.use_intellisense)
                SguiCompletor.instance.ResetIntellisense();

            string text = main_input_field.text;
            if (string.IsNullOrWhiteSpace(text))
            {
                lint_tmp.text = text;
                return;
            }

            BoaReader reader = new(harbinger_view.lint_theme, strict_syntax, text, script_path, cursor_i: main_input_field.caretPosition);
            BoaSignal signal = new(SIG_FLAGS_old.LINT, reader);
            ScopeNode scope = new(null, false);

            Harbinger harbinger = new(null, null, workdir, (data, lint) => Debug.Log(lint ?? data, this));
            harbinger.signal = signal;

            harbinger.TryParseProgram(reader, scope, out _, out _);

            lint_tmp.text = reader.GetLintResult();

            int index_completor_window = reader.cpl_start;
            index_completor_window = main_input_field.caretPosition;
            TMP_CharacterInfo info = main_input_field.textComponent.textInfo.characterInfo[index_completor_window];

            Vector3 worldPos = main_input_field.textComponent.rectTransform.TransformPoint(info.bottomLeft);

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

            if (settings.use_intellisense)
                if (reader.cpl_end > reader.cpl_start)
                    if (reader.completions_v.Count > 0)
                        if (!string.IsNullOrWhiteSpace(reader.text[reader.cpl_start..reader.cpl_end]))
                            SguiCompletor.instance.PopulateCompletions(reader.cpl_start, reader.cpl_end, screenPos, reader.completions_v);
        }
    }
}