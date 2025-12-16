using _COBRA_;
using _SGUI_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        char OnValidateStdin_char(string text, int charIndex, char addedChar)
        {
            switch (shell.status._value)
            {
                case CMD_STATUS.WAIT_FOR_STDIN:
                    try
                    {
                        switch (addedChar)
                        {
                            case '\t':
                                OnTab();
                                return '\0';

                            case '\n':
                                OnSubmit();
                                ResetStdin();
                                return '\0';

                            case ' ':
                                return Util.NOWRAP_CHAR;

                            default:
                                return addedChar;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);
                        return '\0';
                    }

                case CMD_STATUS.BLOCKED:
                    ResetStdin();
                    return '\0';

                case CMD_STATUS.NETWORKING:
                    return '\0';

                default:
                    return '\0';
            }
        }

        void OnStdinChanged(string value)
        {
            if (value.HasSpaces())
            {
                LoggerOverlay.Log($"HAS SPACES (value: [[ {value} ]] [[ {stdin_field.text} ]])", this);
                stdin_field.text = Util.ForceCharacterWrap(value);
                return;
            }

            if (!CheckPrefixe())
                return;

            if (!flag_history.PullValue())
                ResetHistoryNav();

            switch (shell.status._value)
            {
                case CMD_STATUS.WAIT_FOR_STDIN:
                    OnChange();
                    break;

                case CMD_STATUS.BLOCKED:
                    ResetStdin();
                    return;

                case CMD_STATUS.NETWORKING:
                    break;
            }
        }
    }
}