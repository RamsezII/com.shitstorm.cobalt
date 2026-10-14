namespace _COBALT_
{
    partial class ScriptView
    {

        //--------------------------------------------------------------------------------------------------------------

        void InitExecutionButton()
        {
            var button_execution = AddTopRightButton(new() { french = "Lancer", english = "Run", });

            button_execution.button.onClick.AddListener(() => SguiLoggerOverlay.Log($"{this}.Execute: get terminal window to run script at path", this, timer: 10, logLevel: SguiLogLevel.Warning));
        }
    }
}