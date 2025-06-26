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
        [SerializeField] bool use_intellisense;

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnLint()
        {
            base.OnLint();

            if (use_intellisense)
                SguiCompletor.instance.ResetIntellisense();

            string text = main_input_field.text;
            if (string.IsNullOrWhiteSpace(text))
            {
                lint_tmp.text = text;
                return;
            }

            BoaReader reader = new(harbinger_view.lint_theme, strict_syntax, text, script_path, cursor_i: main_input_field.caretPosition);
            BoaSignal signal = new(SIG_FLAGS_new.LINT, reader);
            Harbinger harbinger = new(null, null, workdir, data => Debug.Log(data, this));
            ScopeNode scope = new(null, false);
            harbinger.TryParseProgram(reader, scope, out _, out _);

            lint_tmp.text = reader.GetLintResult();

            TMP_CharacterInfo info = main_input_field.textComponent.textInfo.characterInfo[reader.cpl_start];

            Vector3 worldPos = main_input_field.textComponent.rectTransform.TransformPoint(info.bottomLeft);

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

            if (use_intellisense)
                SguiCompletor.instance.PopulateCompletions(reader.cpl_start, reader.cpl_end, screenPos, reader.completions_v);
        }
    }
}