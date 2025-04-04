using _ARK_;
using _SGUI_;
using _UTIL_;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public partial class Terminal : SguiWindow
    {
        public static Terminal instance;

        public readonly OnValue<bool> isActive = new();

        //--------------------------------------------------------------------------------------------------------------

        static Terminal()
        {
            InitLogs();
        }

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
            Command.cmd_root_shell._commands.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Util.InstantiateOrCreateIfAbsent<Terminal>(SGUI_global.instance.rT);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            tmp_title.SetTrad(typeof(Terminal).Name);

            AwakeUI();

            NUCLEOR.delegates.getInputs += () =>
            {
                lock (isActive)
                    if (!isActive._value)
                        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.T))
                            isActive.Update(true);
            };

            lock (onLog)
                lock (pending_logs)
                {
                    foreach (LogInfos log in pending_logs)
                        AddLine_log(log);
                    pending_logs.Clear();
                    onLog._value = AddLine_log;
                }

            AwakeShell();
        }

        private void OnEnable()
        {
            NUCLEOR.delegates.onLateUpdate -= OnLateUpdate;
            NUCLEOR.delegates.onLateUpdate += OnLateUpdate;

            USAGES.ToggleUser(this, true, UsageGroups.TrueMouse, UsageGroups.Keyboard, UsageGroups.BlockPlayers, UsageGroups.Typing);

            flag_stdout.Update(true);

            ClearStdout();

            NUCLEOR.delegates.onEndOfFrame_once += () => EventSystem.current.SetSelectedGameObject(input_stdin.input_field.gameObject);
        }

        private void OnDisable()
        {
            NUCLEOR.delegates.onLateUpdate -= OnLateUpdate;
            USAGES.RemoveUser(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            StartUI();
            isActive.AddListener(active => gameObject.SetActive(active));

            IMGUI_global.instance.users.RemoveElement(this);
            IMGUI_global.instance.users.AddElement(this);

            NUCLEOR.delegates.getInputs += OnGetInputs;
            NUCLEOR.delegates.onPlayerInputs += OnUpdate;

        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            NUCLEOR.delegates.getInputs -= OnGetInputs;
            NUCLEOR.delegates.onPlayerInputs -= OnUpdate;

            base.OnDestroy();

            if (this == instance)
                instance = null;
        }
    }
}