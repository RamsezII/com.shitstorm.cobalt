using _ARK_;
using _COBRA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        void OnSelectStdin(string text)
        {
            IMGUI_global.instance.inputs_users.AddElement(OnImguiInputs);

            NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () =>
            {
                int min_pos = shell.status._value.prefixe.Text.Length;
                if (stdin_field.caretPosition < min_pos)
                    stdin_field.caretPosition = min_pos;
            };
        }

        void OnDeselectStdin(string arg0)
        {
            IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
        }

        bool GetStdin(out string stdin, out int cursor_i)
        {
            int pref_len = shell.status._value.prefixe.Text.Length;
            stdin = stdin_field.text[pref_len..];
            cursor_i = stdin_field.caretPosition - pref_len;
            Util.RemoveCharacterWrap(ref stdin);
            return !string.IsNullOrWhiteSpace(stdin);
        }

        void ResetStdin()
        {
            LintedString prefixe = shell.status._value.prefixe;
            string pref_text = prefixe.Text;
            string pref_lint = prefixe.Lint;

            Util.ForceCharacterWrap(ref pref_text);
            Util.ForceCharacterWrap(ref pref_lint);

            if (!stdin_field.text.Equals(pref_text, StringComparison.Ordinal))
                stdin_field.text = pref_text;

            stdin_field.lint.text = pref_lint;

            stdin_field.caretPosition = pref_text.Length;

            ResizeStdin();
        }

        void ResizeStdin()
        {
            Rect prect = scrollview.viewport.rect;

            float stdin_h = stdin_field.textComponent.GetInvisibleHeight();

            float bottom_height = content_rT.anchoredPosition.y - stdout_h - stdin_h - offset_bottom_h + prect.height;
            stdin_h = Mathf.Max(stdin_h, prect.height);

            stdin_field.rT.sizeDelta = new(0, stdin_h);
            content_rT.sizeDelta = new(0, stdout_h + stdin_h);

            if (bottom_height < 0)
                content_rT.anchoredPosition += new Vector2(0, -bottom_height);
        }

        bool CheckPrefixe()
        {
            string current = stdin_field.text;
            string prefixe = shell.status._value.prefixe.Text;

            Util.ForceCharacterWrap(ref prefixe);

            if (current.StartsWith(prefixe, StringComparison.Ordinal))
                return true;

            if (current.Length < prefixe.Length)
                current = prefixe;
            else
                current = prefixe + current[prefixe.Length..];

            if (!string.Equals(current, stdout_field.text, StringComparison.Ordinal))
                stdin_field.text = current;

            if (stdin_field.caretPosition < prefixe.Length)
                stdin_field.caretPosition = prefixe.Length;

            return false;
        }
    }
}