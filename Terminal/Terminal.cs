using _ARK_;
using _SGUI_;
using _UTIL_;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _COBALT_
{
    public partial class Terminal : SguiWindow
    {
        public ListListener<Terminal> terminals = new();

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            AwakeUI();

            terminals.AddElement(this);
            terminals.AddListener2(OnTerminalsStack);
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnTerminalsStack(List<Terminal> list)
        {
            NUCLEOR.delegates.onLateUpdate -= PullLogs;
            if (list.Count > 0 && list[^1] == this)
                NUCLEOR.delegates.onLateUpdate += PullLogs;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();

            terminals.RemoveElement(this);
            terminals._listeners2 -= OnTerminalsStack;
        }
    }
}