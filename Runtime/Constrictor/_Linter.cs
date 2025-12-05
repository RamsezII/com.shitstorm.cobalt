using _ARK_;
using _UTIL_;
using _SGUI_;
using TMPro;
using UnityEngine;
using _COBRA_;

namespace _COBALT_
{
    partial class Constrictor
    {
        [SerializeField] bool strict_syntax;
        [SerializeField] string script_path;
        [SerializeField] string workdir;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AddLinter()
        {
            ScriptView.on_stdin_linters.Add("ZOA", script_view =>
            {
                script_view.lint_theme = LintTheme.theme_light;

                string text = script_view.input_field.text;

                CodeReader reader = new(lint_theme: script_view.lint_theme,
                    strict_syntax: false,
                    text: text,
                    script_path: script_view.file_path._value?.FullName ?? null,
                    cursor_i: script_view.input_field.caretPosition
                );

                using _ZOA_.ZoaShell shell = new();
                shell.Init();
                _ZOA_.Signal signal = new(shell, _ZOA_.SIG_FLAGS.CHANGE | _ZOA_.SIG_FLAGS.SCRIPT | _ZOA_.SIG_FLAGS.LINT, reader, static (data, lint) =>
                {

                });
                shell.OnSignal(signal);

                script_view.input_lint.text = signal.reader.GetLintResult();

                if (reader.sig_error != null)
                {
                    int lines = 0;
                    int last_index = 1;

                    for (int i = 0; i < reader.read_i; ++i)
                        if (text[i] == '\n')
                        {
                            last_index = 1 + i;
                            ++lines;
                        }

                    string error = $"└──> {reader.sig_error}";

                    if (reader.read_i > last_index)
                        error = new string(' ', reader.read_i - last_index) + error;

                    if (lines >= 0)
                        error = new string('\n', 1 + lines) + error;

                    script_view.input_error.text = error;
                }
                else
                    script_view.input_error.text = string.Empty;
            });

            ScriptView.on_stdin_linters.Add("BOA", script_view =>
            {
                SguiCompletor.instance.ResetIntellisense();

                string text = script_view.input_field.text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    script_view.input_lint.text = text;
                    return;
                }

                _BOA_.BoaReader reader = new(LintTheme.theme_light,
                    strict_syntax: false,
                    text: text,
                    script_path: script_view.file_path._value.FullName,
                    cursor_i: script_view.input_field.caretPosition
                );

                _BOA_.BoaSignal signal = new(_BOA_.SIG_FLAGS_old.LINT, reader);
                _BOA_.ScopeNode scope = new(null, false);

                _BOA_.Harbinger harbinger = new(null, null, ArkPaths.instance.Value.dpath_home, (data, lint) => Debug.Log(lint ?? data));
                harbinger.signal = signal;

                harbinger.TryParseProgram(reader, scope, out _, out _);

                script_view.input_lint.text = reader.GetLintResult();

                int index_completor_window = reader.cpl_start;
                index_completor_window = script_view.input_field.caretPosition;
                TMP_CharacterInfo info = script_view.input_field.textComponent.textInfo.characterInfo[index_completor_window];

                Vector3 worldPos = script_view.input_field.textComponent.rectTransform.TransformPoint(info.bottomLeft);

                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

                if (reader.cpl_end > reader.cpl_start)
                    if (reader.completions_v.Count > 0)
                        if (!string.IsNullOrWhiteSpace(reader.text[reader.cpl_start..reader.cpl_end]))
                            SguiCompletor.instance.PopulateCompletions(reader.cpl_start, reader.cpl_end, screenPos, reader.completions_v);
            });
        }
    }
}