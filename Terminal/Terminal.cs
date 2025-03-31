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

        public Command shell;

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
            Util.InstantiateOrCreateIfAbsent<Terminal>();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            shell = new Command(prefixe: "~");

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
        }

        private void OnEnable()
        {
            NUCLEOR.delegates.getInputs -= OnGetInputs;
            NUCLEOR.delegates.getInputs += OnGetInputs;

            NUCLEOR.delegates.onPlayerInputs -= OnApplyInputs;
            NUCLEOR.delegates.onPlayerInputs += OnApplyInputs;

            NUCLEOR.delegates.onLateUpdate -= CheckFlags;
            NUCLEOR.delegates.onLateUpdate += CheckFlags;

            USAGES.ToggleUser(this, true, UsageGroups.TrueMouse, UsageGroups.Keyboard, UsageGroups.BlockPlayers);

            flag_stdout.Update(true);

            ClearStdout();

            EventSystem.current.SetSelectedGameObject(input_stdin.input_field.gameObject);
        }

        private void OnDisable()
        {
            NUCLEOR.delegates.getInputs -= OnGetInputs;
            NUCLEOR.delegates.onPlayerInputs -= OnApplyInputs;
            NUCLEOR.delegates.onLateUpdate -= CheckFlags;

            USAGES.RemoveUser(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            StartUI();
            isActive.AddListener(active => gameObject.SetActive(active));
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (this == instance)
                instance = null;
        }
    }
}