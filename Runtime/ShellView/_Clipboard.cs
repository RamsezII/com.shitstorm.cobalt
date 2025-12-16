using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        bool OnClipboardOperation(Event e, IMGUI_global.ClipboardOperations operation)
        {
            var field = stdin_field.isFocused ? stdin_field : stdout_field.isFocused ? stdout_field : null;
            if (field == null)
                return false;

            string selected;
            int start, end;

            switch (operation)
            {
                case IMGUI_global.ClipboardOperations.Copy:
                    if (field.TryGetSelectedString(out selected, out _, out _))
                    {
                        Util.RemoveCharacterWrap(ref selected);
                        GUIUtility.systemCopyBuffer = selected;
                    }
                    return true;

                case IMGUI_global.ClipboardOperations.Cut:
                    if (field.TryGetSelectedString(out selected, out start, out end))
                    {
                        Util.RemoveCharacterWrap(ref selected);
                        GUIUtility.systemCopyBuffer = selected;

                        if (field == stdin_field)
                        {
                            string text = stdin_field.text;
                            text = text[..start] + text[end..];

                            Util.ForceCharacterWrap(ref text);

                            stdin_field.text = text;
                            stdin_field.caretPosition = start;
                        }
                    }
                    return true;

                case IMGUI_global.ClipboardOperations.Paste:
                    if (field == stdin_field)
                    {
                        stdin_field.GetSelectedString(out start, out end);

                        string text = stdin_field.text;
                        text = text[..start] + GUIUtility.systemCopyBuffer + text[end..];

                        Util.ForceCharacterWrap(ref text);

                        stdin_field.text = text;
                        stdin_field.caretPosition = start + GUIUtility.systemCopyBuffer.Length;
                    }
                    return true;
            }

            return false;
        }
    }
}