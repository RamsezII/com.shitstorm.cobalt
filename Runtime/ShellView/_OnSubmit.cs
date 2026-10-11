using System;
using UnityEngine;

namespace _COBALT_
{
    partial class ShellView
    {
        public void ExecuteLine(in string line)
        {
            if (shell == null || shell.IsExecuting)
                return;

            stdin_field.text = Util.ForceCharacterWrap(prefixe_text + line);
            OnSubmit();
        }

        void OnSubmit()
        {
            if (shell == null || shell.IsExecuting || !GetStdin(out string stdin, out _))
                return;

            AddLine(stdin_field.text, stdin_field.lint.text);
            try
            {
                int exitCode = shell.Execute(stdin);
                if (this != null && exitCode == 0)
                    AddToHistory(stdin);
            }
            catch (Exception exception)
            {
                if (this != null)
                {
                    output_writer.Flush();
                    error_writer.Flush();
                    AddLine(exception.Message, exception.Message.SetColor(lint_theme._value.error));
                    Debug.LogException(exception, this);
                }
            }
            finally
            {
                if (this != null)
                {
                    output_writer.Flush();
                    error_writer.Flush();
                    ResetStdin();
                    if (isFocused._value)
                        stdin_field.Select();
                }
            }
        }
    }
}
