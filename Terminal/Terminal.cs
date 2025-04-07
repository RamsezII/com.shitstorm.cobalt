using _ARK_;
using _COBRA_;
using _SGUI_;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _COBALT_
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public partial class Terminal : SguiWindow, ITerminal
    {
        public static Terminal instance;

        public Command.Executor executor;
        Command.Executor ITerminal.RootExecutor => executor;
        void ITerminal.ToggleWindow(bool toggle) => isActive.Update(toggle);

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

            lock (onLog)
                lock (pending_logs)
                {
                    foreach (LogInfos log in pending_logs)
                        AddLine_log(log);
                    pending_logs.Clear();
                    onLog._value = AddLine_log;
                }

            Command.cmd_root_shell.AddCommand(new(
                "exit",
                manual: new("guess what"),
                action: exe => isActive.Update(false)
                ));

            executor = new Command.Executor(null, new() { new("shell_root", Command.cmd_root_shell), }, new Command.Line(string.Empty, CMD_SIGNALS.EXEC, this));
            executor.Executate(new Command.Line(string.Empty, CMD_SIGNALS.EXEC, this));

            IMGUI_global.instance.users_ongui.AddElement(OnOnGui, this);

            hide_stdout.AddListener(toggle => flag_stdout.Update(true));
            hide_stdout.AddListener(toggle => Debug.Log($"{nameof(hide_stdout)}: {toggle}"));
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            NUCLEOR.delegates.onLateUpdate -= OnLateUpdate;
            NUCLEOR.delegates.onLateUpdate += OnLateUpdate;

            USAGES.ToggleUser(this, true, UsageGroups.TrueMouse, UsageGroups.Keyboard, UsageGroups.BlockPlayers, UsageGroups.Typing);

            flag_stdout.Update(true);

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

            NUCLEOR.delegates.getInputs += OnGetInputs;
            NUCLEOR.delegates.onPlayerInputs += OnUpdate;

            fullscreen.AddListener(value => flag_stdout.Update(true));
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            NUCLEOR.delegates.getInputs -= OnGetInputs;
            NUCLEOR.delegates.onPlayerInputs -= OnUpdate;

            base.OnDestroy();

            if (this == instance)
                instance = null;

            IMGUI_global.instance.users_ongui.RemoveElement(this);
        }
    }
}