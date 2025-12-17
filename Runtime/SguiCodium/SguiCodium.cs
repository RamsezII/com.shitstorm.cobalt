using _SGUI_;
using UnityEngine;

namespace _COBALT_
{
    public partial class SguiCodium : SguiWindow1
    {

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
            base.OnAwake();
        }
    }
}