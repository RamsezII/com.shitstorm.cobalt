using _BOA_;

namespace _COBALT_
{
    partial class ScriptView
    {

        //--------------------------------------------------------------------------------------------------------------

        void InitExecutionButton()
        {
            var button_execution = header.AddButton(new() { french = "Compiler", english = "Compile", });
            button_execution.button.onClick.AddListener(static () => BoaManager.CompileScripts());
        }
    }
}