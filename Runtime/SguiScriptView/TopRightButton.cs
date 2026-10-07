using _ARK_;
using _SGUI_;
using _SGUI_.context_click;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace _COBALT_.scriptview
{
    internal sealed partial class TopRightButton : MonoBehaviour, SguiContextList.IUser_LeftClick
    {
        public Button button;
        public Traductable trad_label;
        public Action<ContextList> onList;
        void SguiContextList.IUser.OnSguiContextClick(ContextList list) => onList?.Invoke(list);

        //--------------------------------------------------------------------------------------------------------------

    }
}