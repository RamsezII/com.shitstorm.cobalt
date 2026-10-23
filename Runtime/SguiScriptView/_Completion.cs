using _ARK_;
using _BOA_;
using _SGUI_;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        CancellationTokenSource completionCancellation;
        CodeCompletionResult completionResult;
        bool committingCompletion;
        string completionText, completionPath;
        int completionCursor = -1;

        //----------------------------------------------------------------------------------------------------------

        void LateUpdate()
        {
            RefreshScrollView();

            if (codeActionsMenu != null || committingCodeAction)
            {
                if (!isFocused._value)
                    CancelCodeActions();
                CancelCompletion();
                return;
            }

            if (committingCompletion)
            {
                if (!use_intellisense || !isFocused._value || input_field.text != completionText || input_field.selectionStringFocusPosition != completionCursor || input_field.selectionStringAnchorPosition != completionCursor || file?.FullName != completionPath)
                    CancelCompletion();
                return;
            }

            if (!use_intellisense || !input_field.isFocused || !isFocused._value || input_field.selectionStringAnchorPosition != input_field.selectionStringFocusPosition)
            {
                CancelCompletion();
                completionCursor = -1;
                return;
            }

            int cursor = input_field.selectionStringFocusPosition;
            if (completionText == input_field.text && completionCursor == cursor && completionPath == file?.FullName)
                return;

            RefreshCompletions();
        }

        async void RefreshCompletions(bool force = false)
        {
            CancelCompletion();
            completionText = input_field.text;
            completionCursor = input_field.selectionStringFocusPosition;
            completionPath = file?.FullName;

            if (!use_intellisense || !input_field.isFocused || input_field.selectionStringAnchorPosition != completionCursor || SguiCompletor.instance == null)
                return;
            if (!force && (completionCursor == 0 || !(char.IsLetterOrDigit(completionText[completionCursor - 1]) || completionText[completionCursor - 1] is '_' or '.' or '@' or ' ' or ':')))
                return;

            var request = completionCancellation = new CancellationTokenSource();
            var token = request.Token;
            string text = completionText, path = completionPath, directory = GetScriptsDir().FullName;
            int cursor = completionCursor;
            CodeCompletionResult result = null;

            try
            {
                if (!force)
                    await Task.Delay(120, token);
                result = await BoaCode.CompleteCodeAsync(text, cursor, path, directory, token, explicitInvocation: force);
                if (token.IsCancellationRequested || this == null || !isActiveAndEnabled || !input_field.isFocused || !isFocused._value || input_field.text != text || input_field.selectionStringFocusPosition != cursor || input_field.selectionStringAnchorPosition != cursor || file?.FullName != path)
                    return;
                if (result.Suggestions.Length == 0)
                    return;

                input_field.ForceLabelUpdate();
                var textInfo = input_field.textComponent.textInfo;
                if (textInfo.characterCount == 0)
                    return;
                int index = Mathf.Clamp(input_field.caretPosition, 0, textInfo.characterCount - 1);
                var character = textInfo.characterInfo[index];
                float x = input_field.caretPosition < textInfo.characterCount ? character.origin : character.xAdvance;
                Vector3 position = input_field.textComponent.transform.TransformPoint(new Vector3(x, character.descender, 0));
                completionResult = result;
                SguiCompletor.instance.PopulateCompletions(result.Start, result.End, position, result.Suggestions.Take(100), this, value => ApplyCompletion(value));
                result = null;
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
                result?.Dispose();
                if (completionCancellation == request)
                    completionCancellation = null;
                request.Dispose();
            }
        }

        bool ApplyCompletion(string value)
        {
            var completor = SguiCompletor.instance;
            if (completor == null || completionResult == null || !ReferenceEquals(completor.owner, this) || input_field.text != completionText || input_field.selectionStringFocusPosition != completionCursor || input_field.selectionStringAnchorPosition != completionCursor)
                return false;

            var result = completionResult;
            completionResult = null;
            CancelCompletion();
            var request = completionCancellation = new CancellationTokenSource();
            committingCompletion = true;
            CommitCompletion(result, value, request, completionText, completionCursor, completionPath);
            return true;
        }

        async void CommitCompletion(CodeCompletionResult result, string value, CancellationTokenSource request, string source, int sourceCursor, string path)
        {
            var token = request.Token;
            try
            {
                var edit = await result.GetChangeAsync(value, token);
                if (token.IsCancellationRequested || this == null || !isActiveAndEnabled || !isFocused._value || input_field.text != source || input_field.selectionStringFocusPosition != sourceCursor || input_field.selectionStringAnchorPosition != sourceCursor || file?.FullName != path)
                    return;

                input_field.SetTextWithoutNotify(edit.Text);
                input_field.stringPosition = edit.Cursor;
                input_field.ForceLabelUpdate();
                completionText = edit.Text;
                completionCursor = edit.Cursor;
                OnValueChanged(edit.Text);
                if (!input_field.isFocused)
                {
                    input_field.ActivateInputField();
                    NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () =>
                    {
                        if (this != null)
                            input_field.stringPosition = edit.Cursor;
                    };
                }
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
                if (completionCancellation == request)
                {
                    completionCancellation = null;
                    committingCompletion = false;
                }
                request.Dispose();
                result.Dispose();
            }
        }

        void CancelCompletion()
        {
            completionCancellation?.Cancel();
            completionCancellation = null;
            committingCompletion = false;
            completionResult?.Dispose();
            completionResult = null;
            if (SguiCompletor.instance != null && ReferenceEquals(SguiCompletor.instance.owner, this))
                SguiCompletor.instance.ResetIntellisense();
        }

        protected override void OnDisable()
        {
            CancelCodeActions();
            CancelDiagnostics();
            NUCLEOR.delegates.LateUpdate_onEndOfFrame_once -= RefreshInputField_now;
            CancelCompletion();
            completionCursor = -1;
            base.OnDisable();
        }
    }
}
