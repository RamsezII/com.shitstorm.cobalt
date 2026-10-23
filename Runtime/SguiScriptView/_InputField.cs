using _ARK_;
using _SGUI_;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        CancellationTokenSource diagnosticsCancellation;

        //--------------------------------------------------------------------------------------------------------------

        void InitInputField()
        {
            input_field.onValueChanged.AddListener(OnValueChanged);
            input_field.onValidateInput += OnValidateInput;
            input_field.onFocusSelectAll = false;
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnValueChanged(string text)
        {
            if (!dirty)
            {
                dirty = true;
                RefreshTitle();
            }
            RefreshInputField();
        }

        void RefreshInputField()
        {
            CancelDiagnostics();
            Util.AddActionOnce(ref NUCLEOR.delegates.LateUpdate_onEndOfFrame_once, RefreshInputField_now);
        }

        void RefreshInputField_now()
        {
            if (this == null || !isActiveAndEnabled || !current_theme.Has)
                return;

            string text = input_field.text;

            graphic_background.color = current_theme._value.background;
            input_field.caretColor = current_theme._value.cursor;
            text_placeholder.color = current_theme._value.placeholder;

            text_lint.text = _BOA_.Shell.HighlightCode(text, current_theme._value);
            text_error.text = string.Empty;
            text_error.gameObject.SetActive(false);
            RefreshDiagnostics(text, current_theme._value);
        }

        async void RefreshDiagnostics(string text, LintTheme theme)
        {
            var request = diagnosticsCancellation = new CancellationTokenSource();
            var token = request.Token;
            try
            {
                await Task.Delay(250, token);
                token.ThrowIfCancellationRequested();
                string error = await _BOA_.Shell.GetCodeDiagnosticAsync(text, theme, token);
                if (token.IsCancellationRequested || this == null || !isActiveAndEnabled || diagnosticsCancellation != request || input_field.text != text)
                    return;

                bool hasError = !string.IsNullOrWhiteSpace(error);
                text_error.text = hasError ? $"{new string(' ', text.Length)}{error}" : string.Empty;
                text_error.gameObject.SetActive(hasError);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                if (this != null && !token.IsCancellationRequested)
                    Debug.LogException(exception, this);
            }
            finally
            {
                if (diagnosticsCancellation == request)
                    diagnosticsCancellation = null;
                request.Dispose();
            }
        }

        void CancelDiagnostics()
        {
            diagnosticsCancellation?.Cancel();
            diagnosticsCancellation = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (current_theme.Has)
                RefreshInputField();
        }

        char OnValidateInput(string text, int charIndex, char addedChar)
        {
            var completor = SguiCompletor.instance;
            if (completor != null && completor.toggle.Value && ReferenceEquals(completor.owner, this))
                switch (addedChar)
                {
                    case ' ' when space_confirms_completion:
                    case '\n':
                    case '\t':
                        {
                            string completion = completor.GetSelectedValue();
                            if (!string.IsNullOrWhiteSpace(completion) && ApplyCompletion(completion))
                                return '\0';
                            CancelCompletion();
                        }
                        break;
                }
            return addedChar;
        }
    }
}
