using _ARK_;
using _SGUI_;
using _UTIL_;
using System.Collections.Generic;
using UnityEngine;

namespace _COBALT_
{
    public partial class Terminal : SguiWindow
    {
        public static readonly ListListener<Terminal> terminals = new();
        public bool HasFocus => terminals.IsLast(this);

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            lock (logs_queue)
                logs_queue.Clear();
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            tmp_title.SetTrad(typeof(Terminal).FullName);

            AwakeUI();
            terminals.AddElement(this);
            terminals.AddListener2(OnTerminalList);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            refreshLines.AddListener(flag =>
            {
                refreshLines._value = false;
                NUCLEOR.delegates.onEndOfFrame -= RefreshLines;
                NUCLEOR.delegates.onEndOfFrame += RefreshLines;
            });

            NUCLEOR.instance.subScheduler.AddRoutine(Util.EWaitForFrames(10, () => USAGES.ToggleUser(this, true, UsageGroups.TrueMouse, UsageGroups.Keyboard, UsageGroups.BlockPlayers)));
        }

        //--------------------------------------------------------------------------------------------------------------

        public static bool TryGetActiveTerminal(out Terminal terminal) => (terminal = GetActiveTerminal()) != null;
        public static Terminal GetActiveTerminal()
        {
            lock (terminals)
                return terminals._list.Count > 0 ? terminals._list[^1] : null;
        }

        void OnTerminalList(List<Terminal> list)
        {
            if (HasFocus)
                PullLogs();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();
            terminals.RemoveElement(this);
            terminals._listeners2 -= OnTerminalList;
            USAGES.RemoveUser(this);
        }
    }
}