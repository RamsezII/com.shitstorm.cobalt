using _ARK_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        void OnSelectStdin(string text)
        {
            if (IMGUI_global.instance != null)
                IMGUI_global.instance.inputs_users.AddElement(OnImguiInputs);

            NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () =>
            {
                if (this == null || shell == null)
                    return;

                int min_pos = prefixe_text.Length;
                if (stdin_field.caretPosition < min_pos)
                    stdin_field.caretPosition = min_pos;
            };
        }

        void OnDeselectStdin(string arg0)
        {
            if (IMGUI_global.instance != null)
                IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
        }

        bool GetStdin(out string stdin, out int cursor_i)
        {
            int pref_len = Math.Min(prefixe_text.Length, stdin_field.text.Length);
            stdin = stdin_field.text[pref_len..];
            cursor_i = Mathf.Clamp(stdin_field.caretPosition - pref_len, 0, stdin.Length);
            Util.RemoveCharacterWrap(ref stdin);
            return !string.IsNullOrWhiteSpace(stdin);
        }

        void ResetStdin()
        {
            string user = NUCLEOR.instance.user_name._value;
            string workdir = shell?.WorkingDirectory ?? NUCLEOR.DFHome.FullName;
            prefixe_text = $"{user}:{workdir}$ ";
            prefixe_lint = $"{user.SetColor("#73CC26")}:{workdir.SetColor("#73B2D9")}$ ";
            string pref_text = prefixe_text;
            string pref_lint = prefixe_lint;
            applying_completion = completion_active = false;
            last_completion = default;
            stdin_field.interactable = shell != null && !shell.IsExecuting;
            if (tmp_progress != null)
                tmp_progress.text = shell != null && shell.IsExecuting ? "…" : string.Empty;

            Util.ForceCharacterWrap(ref pref_text);
            Util.ForceCharacterWrap(ref pref_lint);

            if (!stdin_field.text.Equals(pref_text, StringComparison.Ordinal))
                stdin_field.text = pref_text;

            stdin_field.lint.text = pref_lint;

            stdin_field.caretPosition = pref_text.Length;

            ResizeStdin();
        }

        internal void ResizeStdin()
        {
            Rect prect = scrollview.viewport.rect;

            float stdin_h = stdin_field.textComponent.GetInvisibleHeight();

            float bottom_height = scrollview.content.anchoredPosition.y - stdout_h - stdin_h - offset_bottom_h + prect.height;
            stdin_h = Mathf.Max(stdin_h, prect.height);

            stdin_field.rT.sizeDelta = new(0, stdin_h);
            scrollview.content.sizeDelta = new(0, stdout_h + stdin_h);

            if (bottom_height < 0)
                scrollview.content.anchoredPosition += new Vector2(0, -bottom_height);
        }

        bool CheckPrefixe()
        {
            string current = stdin_field.text;
            string prefixe = prefixe_text;

            Util.ForceCharacterWrap(ref prefixe);

            if (current.StartsWith(prefixe, StringComparison.Ordinal))
                return true;

            if (current.Length < prefixe.Length)
                current = prefixe;
            else
                current = prefixe + current[prefixe.Length..];

            if (!string.Equals(current, stdin_field.text, StringComparison.Ordinal))
                stdin_field.text = current;

            if (stdin_field.caretPosition < prefixe.Length)
                stdin_field.caretPosition = prefixe.Length;

            return false;
        }
    }
}
