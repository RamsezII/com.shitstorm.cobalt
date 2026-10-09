using _ARK_;
using _SGUI_;
using _SGUI_.context_click;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _COBALT_.scriptview
{
    internal sealed partial class TopRightButton : MonoBehaviour, SguiContextList.IUser
    {
        public Button button;
        public Traductable trad_label;
        public Action<ContextList> onList;
        bool SguiContextList.IUser.AcceptsLeftClick => true;
        bool SguiContextList.IUser.AcceptsRightClick => false;
        void SguiContextList.IUser.OnSguiContextClick(PointerEventData eventData, ContextList list) => onList?.Invoke(list);

        //--------------------------------------------------------------------------------------------------------------

    }
}