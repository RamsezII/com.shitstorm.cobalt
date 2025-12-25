using _SGUI_;
using _UTIL_;
using System.IO;

namespace _COBALT_
{
    partial class ScriptView
    {
        const int MAX_FILE_SIZE = 1024;
        public readonly ValueHandler<FileInfo> file_path = new();

        //--------------------------------------------------------------------------------------------------------------

        void StartFileLoading()
        {
            file_path.AddListener(file =>
            {
                if (file == null || !file.Exists)
                {
                    input_field.text = string.Empty;
                    input_lint.text = string.Empty;
                }
                else if (file.Length <= MAX_FILE_SIZE)
                    input_field.text = File.ReadAllText(file.FullName);
                else
                {
                    SguiCustom sgui = SguiWindow.InstantiateWindow<SguiCustom>();
                    var alert = sgui.AddButton<SguiCustom_Alert>();
                    alert.SetText(new($"{GetType().FullName} : file to big ({file.Length.LogDataSize()})\n{file_path.ToSubLog()}"));
                }
            });
        }
    }
}