using _ARK_;
using _COBRA_;
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

        public Command.Executor executor;

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

            Command.cmd_root_shell.AddCommand("exit", new Command(
                manual: new("guess what"),
                action: exe => isActive.Update(false)
                ));

            executor = new(new() { new("shell_root", Command.cmd_root_shell), }, Command.Line.EMPTY_EXE);
            executor.Executate(Command.Line.EMPTY_EXE);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            NUCLEOR.delegates.onLateUpdate -= OnLateUpdate;
            NUCLEOR.delegates.onLateUpdate += OnLateUpdate;

            USAGES.ToggleUser(this, true, UsageGroups.TrueMouse, UsageGroups.Keyboard, UsageGroups.BlockPlayers, UsageGroups.Typing);

            flag_stdout.Update(true);

            ClearStdout();

            NUCLEOR.delegates.onEndOfFrame_once += () => EventSystem.current.SetSelectedGameObject(input_stdin.input_field.gameObject);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

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