using _SGUI_;
using _UTIL_;
using System;
using System.IO;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        const int MAX_FILE_SIZE = 1024;
        public readonly ValueNotifier<FileInfo> file_path = new();

        //--------------------------------------------------------------------------------------------------------------

        void StartFileLoading()
        {
            file_path.AddListener(file =>
            {
                try
                {
                    file?.Refresh();

                    if (file == null || !file.Exists)
                    {
                        input_field.text = string.Empty;
                        input_lint.text = string.Empty;
                        input_error.text = string.Empty;
                    }
                    else if (file.Length <= MAX_FILE_SIZE)
                        input_field.text = File.ReadAllText(file.FullName);
                    else
                    {
                        SguiCustom sgui = SguiWindow.CreatePrompt();
                        var alert = sgui.AddButton<SguiCustom_Alert>();
                        alert.SetText(new($"{GetType().FullName} : file too big ({file.Length.LogDataSize()})\n{file_path.ToSubLog()}"));
                    }
                }
                catch (Exception exception)
                {
                    input_error.text = exception.Message;
                    Debug.LogException(exception, this);
                }
            });
        }

        //--------------------------------------------------------------------------------------------------------------

        void SaveCurrentFile()
        {
            FileInfo file = file_path._value;
            if (file == null)
            {
                SguiLoggerOverlay.Log($"No file selected for {this}", this, timer: 5);
                return;
            }

            try
            {
                File.WriteAllText(file.FullName, input_field.text);
                file.Refresh();
                SguiLoggerOverlay.Log($"Saved '{file.FullName}'", this, timer: 5);
            }
            catch (Exception exception)
            {
                input_error.text = exception.Message;
                Debug.LogException(exception, this);
            }
        }
    }
}
