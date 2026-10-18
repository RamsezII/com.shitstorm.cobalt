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
        string completionText, completionPath;
        int completionCursor = -1;

        //----------------------------------------------------------------------------------------------------------

        void LateUpdate()
        {
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
            if (!force && (completionCursor == 0 || !(char.IsLetterOrDigit(completionText[completionCursor - 1]) || completionText[completionCursor - 1] is '_' or '.' or '@')))
                return;

            var request = completionCancellation = new CancellationTokenSource();
            var token = request.Token;
            string text = completionText, path = completionPath, directory = GetScriptsDir().FullName;
            int cursor = completionCursor;

            try
            {
                if (!force)
                    await Task.Delay(120, token);
                var result = await Shell.CompleteCodeAsync(text, cursor, path, directory, token);
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
                SguiCompletor.instance.PopulateCompletions(result.Start, result.End, position, result.Suggestions.Take(100), this, value => ApplyCompletion(value));
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
                    completionCancellation = null;
                request.Dispose();
            }
        }

        bool ApplyCompletion(string value)
        {
            var completor = SguiCompletor.instance;
            if (completor == null || !ReferenceEquals(completor.owner, this) || input_field.text != completionText || input_field.selectionStringFocusPosition != completionCursor || input_field.selectionStringAnchorPosition != completionCursor)
                return false;

            string text = input_field.text[..completor.compl_start] + value + input_field.text[completor.compl_end..];
            int cursor = completor.compl_start + value.Length;
            CancelCompletion();
            input_field.SetTextWithoutNotify(text);
            input_field.stringPosition = cursor;
            input_field.ForceLabelUpdate();
            completionText = text;
            completionCursor = cursor;
            OnValueChanged(text);
            if (!input_field.isFocused)
            {
                input_field.ActivateInputField();
                NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () =>
                {
                    if (this != null)
                        input_field.stringPosition = cursor;
                };
            }
            return true;
        }

        void CancelCompletion()
        {
            completionCancellation?.Cancel();
            completionCancellation = null;
            if (SguiCompletor.instance != null && ReferenceEquals(SguiCompletor.instance.owner, this))
                SguiCompletor.instance.ResetIntellisense();
        }

        protected override void OnDisable()
        {
            CancelCompletion();
            completionCursor = -1;
            base.OnDisable();
        }
    }
}
