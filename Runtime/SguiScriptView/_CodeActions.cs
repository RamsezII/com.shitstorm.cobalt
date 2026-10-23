using _ARK_;
using _BOA_;
using _SGUI_;
using _SGUI_.context_click;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
    partial class ScriptView
    {
        CancellationTokenSource codeActionsCancellation;
        CodeActionsResult codeActionsResult;
        ContextList codeActionsMenu;
        bool committingCodeAction;

        void InitCodeActions()
        {
            var handler = input_field.GetComponent<ContextListHandler>() ?? input_field.gameObject.AddComponent<ContextListHandler>();
            handler.callback += OnCodeContextClick;
        }

        void OnCodeContextClick(PointerEventData eventData, ContextList list)
        {
            TakeFocus();
            input_field.ForceLabelUpdate();
            int index = TMP_TextUtilities.GetCursorIndexFromPosition(input_field.textComponent, eventData.position, eventData.pressEventCamera, out var side);
            var info = input_field.textComponent.textInfo;
            int cursor = info.characterCount == 0 ? 0 : info.characterInfo[index].index + (side == CaretPosition.Right ? info.characterInfo[index].stringLength : 0);
            cursor = Mathf.Clamp(cursor, 0, input_field.text.Length);
            int anchor = input_field.selectionStringAnchorPosition, focus = input_field.selectionStringFocusPosition;
            if (cursor < Mathf.Min(anchor, focus) || cursor > Mathf.Max(anchor, focus) || anchor == focus)
                input_field.stringPosition = cursor;
            PopulateCodeActions(list, cursor);
        }

        void ShowCodeActions()
        {
            if (SguiContextList.instance == null)
                return;
            input_field.ForceLabelUpdate();
            var info = input_field.textComponent.textInfo;
            Vector3 position = input_field.textComponent.transform.position;
            if (info.characterCount > 0)
            {
                var character = info.characterInfo[Mathf.Clamp(input_field.caretPosition, 0, info.characterCount - 1)];
                position = input_field.textComponent.transform.TransformPoint(new Vector3(character.origin, character.descender, 0));
            }
            PopulateCodeActions(SguiContextList.instance.InstantiateListAtWorldPoint(position), input_field.selectionStringFocusPosition);
        }

        async void PopulateCodeActions(ContextList list, int cursor)
        {
            CancelCodeActions();
            CancelCompletion();
            codeActionsMenu = list;
            list.onDestroy += () =>
            {
                if (ReferenceEquals(codeActionsMenu, list))
                    CancelCodeActions();
            };

            string source = input_field.text, path = file?.FullName;
            int anchor = input_field.selectionStringAnchorPosition, focus = input_field.selectionStringFocusPosition;
            AddCommand(new() { french = "Couper", english = "Cut" }, KeyCode.X, "Ctrl+X", anchor != focus);
            AddCommand(new() { french = "Copier", english = "Copy" }, KeyCode.C, "Ctrl+C", anchor != focus);
            AddCommand(new() { french = "Coller", english = "Paste" }, KeyCode.V, "Ctrl+V", !string.IsNullOrEmpty(GUIUtility.systemCopyBuffer));
            AddCommand(new() { french = "Tout sélectionner", english = "Select all" }, KeyCode.A, "Ctrl+A", source.Length > 0);
            list.AddLine();
            list.AddButton_trad(new() { french = "Formater le document", english = "Format document" })._button.onClick.AddListener(FormatDocument);
            list.AddLine();
            var first = list.AddButton_trad(new() { french = "Recherche des corrections…", english = "Finding corrections…" });
            first._button.interactable = false;

            var request = codeActionsCancellation = new CancellationTokenSource();
            var token = request.Token;
            CodeActionsResult result = null;
            try
            {
                result = await Shell.GetCodeActionsAsync(source, cursor, path, GetScriptsDir().FullName, token);
                if (token.IsCancellationRequested || this == null || !isActiveAndEnabled || list == null || codeActionsMenu != list || input_field.text != source || file?.FullName != path)
                    return;

                if (result.Suggestions.Length == 0)
                    first.label_left.SetTraductions(new() { french = "Aucune correction à cet endroit", english = "No correction here" });
                else
                    for (int i = 0; i < result.Suggestions.Length; i++)
                    {
                        int index = i;
                        var button = i == 0 ? first : list.AddButton_string(result.Suggestions[i]);
                        button.label_left.SetText(result.Suggestions[i]);
                        foreach (var label in button.label_left.GetComponentsInChildren<TMP_Text>(true))
                            label.richText = false;
                        button._button.interactable = true;
                        button._button.onClick.AddListener(() => ApplyCodeAction(index, source, path));
                    }
                codeActionsResult = result;
                result = null;
                list.AutoSizeAndMove();
            }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                if (this != null && !token.IsCancellationRequested)
                {
                    if (first != null)
                        first.label_left.SetTraductions(new() { french = "Corrections indisponibles", english = "Corrections unavailable" });
                    Debug.LogException(exception, this);
                }
            }
            finally
            {
                result?.Dispose();
                if (codeActionsCancellation == request)
                    codeActionsCancellation = null;
                request.Dispose();
            }

            void AddCommand(Traductions title, KeyCode key, string shortcut, bool enabled)
            {
                var button = list.AddButton_trad(title);
                button.label_right.SetText(shortcut);
                button._button.interactable = enabled;
                button._button.onClick.AddListener(() =>
                {
                    if (this == null || input_field.text != source)
                        return;
                    input_field.selectionStringAnchorPosition = anchor;
                    input_field.selectionStringFocusPosition = focus;
                    input_field.ProcessEvent(new Event { type = EventType.KeyDown, keyCode = key, modifiers = EventModifiers.Control });
                    RestoreCodeSelection(input_field.selectionStringAnchorPosition, input_field.selectionStringFocusPosition);
                });
            }
        }

        async void ApplyCodeAction(int index, string source, string path)
        {
            var result = codeActionsResult;
            codeActionsResult = null;
            CancelCodeActions();
            if (result == null)
                return;

            var request = codeActionsCancellation = new CancellationTokenSource();
            var token = request.Token;
            committingCodeAction = true;
            try
            {
                var edit = await result.GetChangeAsync(index, token);
                if (token.IsCancellationRequested || this == null || !isActiveAndEnabled || input_field.text != source || file?.FullName != path)
                    return;
                input_field.SetTextWithoutNotify(edit.Text);
                RestoreCodeSelection(edit.Cursor, edit.Cursor);
                OnValueChanged(edit.Text);
            }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                if (this != null && !token.IsCancellationRequested)
                    Debug.LogException(exception, this);
            }
            finally
            {
                if (codeActionsCancellation == request)
                {
                    codeActionsCancellation = null;
                    committingCodeAction = false;
                }
                request.Dispose();
                result.Dispose();
            }
        }

        void RestoreCodeSelection(int anchor, int focus)
        {
            TakeFocus();
            input_field.ActivateInputField();
            input_field.selectionStringAnchorPosition = anchor;
            input_field.selectionStringFocusPosition = focus;
            input_field.ForceLabelUpdate();
            string source = input_field.text;
            NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () =>
            {
                if (this == null || !isActiveAndEnabled || input_field.text != source)
                    return;
                input_field.selectionStringAnchorPosition = anchor;
                input_field.selectionStringFocusPosition = focus;
            };
        }

        void CancelCodeActions()
        {
            codeActionsCancellation?.Cancel();
            codeActionsCancellation = null;
            committingCodeAction = false;
            codeActionsResult?.Dispose();
            codeActionsResult = null;
            if (codeActionsMenu != null)
                Destroy(codeActionsMenu.gameObject);
            codeActionsMenu = null;
        }
    }
}
