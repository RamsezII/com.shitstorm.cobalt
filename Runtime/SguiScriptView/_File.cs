using _ARK_;
using _BOA_;
using _SGUI_;
using _SGUI_.context_click;
using System;
using System.IO;
using UnityEngine;

namespace _COBALT_
{
    partial class ScriptView
    {
        const int MAX_FILE_SIZE = 1024;
        internal FileInfo file;
        public static DirectoryInfo GetScriptsDir() => Path.Combine(NUCLEOR.DFHome.FullName, "scripts").ForceDir();

        //--------------------------------------------------------------------------------------------------------------

        void InitFileButton()
        {
            var button_file = header.AddButton(new() { french = "Fichier", english = "File", });
            button_file.onList += (ContextList list) =>
            {
                if (file != null)
                {
                    var button_save = list.AddButton_trad(new() { french = "Sauvegarder", english = "Save", });
                    button_save.label_right.SetText("Ctrl+S");
                    button_save.action += SaveCurrentFile;
                }

                var button_saveAs = list.AddButton_trad(new() { french = "Sauvegarder sous", english = "Save as", });
                button_saveAs.label_right.SetTraductions(new() { french = "Ctrl+Maj+S", english = "Ctrl+Shift+S", });
                button_saveAs.action += SaveAs;
            };
        }

        internal bool LoadFile(in FileInfo file, bool onlyIfChanged = false)
        {
            file.Refresh();
            if (!file.Exists)
                throw new FileNotFoundException("Script file not found.", file.FullName);

            if (file.Length > MAX_FILE_SIZE)
            {
                SguiCustom sgui = SguiCustom.CreatePrompt();
                var alert = sgui.AddButton<SguiCustom_Alert>();
                alert.SetText(new($"{GetType().FullName} : file too big ({file.Length.LogDataSize()})\n{file.FullName.ToSubLog()}"));
                return false;
            }

            string content = File.ReadAllText(file.FullName);
            if (onlyIfChanged && this.file?.FullName == file.FullName && input_field.text == content)
                return true;

            this.file = file;
            input_field.SetTextWithoutNotify(content);
            text_lint.text = string.Empty;
            text_error.text = string.Empty;
            dirty = false;
            RefreshTitle();
            RefreshInputField();
            return true;
        }

        //--------------------------------------------------------------------------------------------------------------

        void ReloadCurrentFile()
        {
            if (file == null || dirty)
                return;

            try
            {
                file.Refresh();
                if (file.Exists)
                    LoadFile(file, onlyIfChanged: true);
            }
            catch (Exception exception)
            {
                text_error.text = exception.Message;
                Debug.LogException(exception, this);
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        void SaveAs()
        {
            SguiCustom.ShowAlert(SguiDialogs.Info, out _, new("Save as is not implemented."));
        }

        void SaveCurrentFile()
        {
            FormatDocument();

            if (file == null)
            {
                SguiLoggerOverlay.Log($"No file selected for {this}", this, logLevel: SguiLogLevel.Warning);
                return;
            }

            try
            {
                File.WriteAllText(file.FullName, input_field.text);
                file.Refresh();
                SguiLoggerOverlay.Log($"Saved '{file.FullName}'", this, logLevel: SguiLogLevel.Sublog);
                dirty = false;
                RefreshTitle();
                BoaManager.CompileScripts();
            }
            catch (Exception exception)
            {
                text_error.text = exception.Message;
                Debug.LogException(exception, this);
            }
        }
    }
}
