using _SGUI_;
using UnityEngine;

namespace _COBALT_
{
    public sealed partial class SguiTerminal : SguiWindow1
    {
        public HarbingerView harbinger_view;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            OSView.instance.AddOrGetSoftwareButton<SguiTerminal>();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            harbinger_view = GetComponentInChildren<HarbingerView>();
            base.Awake();
        }
    }
}