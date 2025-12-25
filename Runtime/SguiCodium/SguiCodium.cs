using _SGUI_;
using UnityEngine;

namespace _COBALT_
{
    public partial class SguiCodium : SguiWindow1
    {
        public ScriptView scriptview;
        public ShellView shellview;
        public SguiExplorerView explorerview;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            OSView.instance.AddSoftwareButton<SguiCodium>(new()
            {
                french = $"Éditeur de code",
                english = $"Code editor",
            });
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            scriptview = GetComponentInChildren<ScriptView>(true);
            shellview = GetComponentInChildren<ShellView>(true);
            explorerview = GetComponentInChildren<SguiExplorerView>(true);

            base.OnAwake();

            trad_title.SetTrad("ShitCodium");
        }
    }
}