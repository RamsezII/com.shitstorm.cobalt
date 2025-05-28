using _COBRA_;
using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        char OnValidateStdin(string text, int charIndex, char addedChar)
        {
            Command.Signal.ResetHistoryCount();
            flag_stdin.Update(true);

            if (shell.current_status.state == CMD_STATES.BLOCKING)
                return '\0';

            try
            {
                switch (addedChar)
                {
                    case '\t':
                        OnTab(charIndex);
                        return '\0';

                    case '\n':
                        OnSubmit();
                        return '\0';
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return '\0';
            }
            return addedChar;
        }
    }
}