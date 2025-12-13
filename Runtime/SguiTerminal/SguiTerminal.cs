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
            OSView.instance.AddSoftwareButton<SguiTerminal>(new("Harbinger terminal"));
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            harbinger_view = GetComponentInChildren<HarbingerView>();
            base.OnAwake();
            trad_title.SetTrad("HARBINGER");
        }
    }
}