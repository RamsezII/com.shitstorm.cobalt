using _ARK_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        char OnValidateStdin_char(string text, int charIndex, char addedChar)
        {
            if (shell == null || shell.IsExecuting)
                return '\0';

            try
            {
                switch (addedChar)
                {
                    case '\t':
                        OnTab();
                        return '\0';
                    case '\n':
                        OnSubmit();
                        return '\0';
                    case ' ':
                        return Util.NOWRAP_CHAR;
                    default:
                        return addedChar;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                return '\0';
            }
        }

        void OnStdinChanged(string value)
        {
            if (value.HasSpaces())
            {
                stdin_field.text = Util.ForceCharacterWrap(value);
                return;
            }

            if (!CheckPrefixe())
                return;

            if (!flag_history.PullValue())
                ResetHistoryNav();

            if (shell == null || shell.IsExecuting)
                return;

            OnChange();
        }
    }
}
