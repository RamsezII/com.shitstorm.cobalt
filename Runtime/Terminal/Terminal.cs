using _ARK_;
using _COBRA_;
using _SGUI_;
using _UTIL_;
using UnityEngine;

namespace _COBALT_
{
    public partial class Terminal : SguiWindow1, ITerminal
    {
        public static Terminal instance;

        public Shell shell;
        Shell ITerminal.GetShell => shell;
        void ITerminal.ToggleWindow(bool toggle) => sgui_toggle_window.Update(toggle);
        public override SoftwareButton taskbar_button => SguiGlobal.instance.button_terminal;

        public readonly OnValue<KeyCode>
            flag_alt = new(),
            flag_nav_history = new();

        [SerializeField] string stdin_save;
        [SerializeField] int cpl_index;
        [SerializeField] int stdin_frame, tab_frame;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Util.InstantiateOrCreateIfAbsent<Terminal>(SguiGlobal.instance.rT_2D);
            InitSoftwareButton();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            instance = this;

            base.Awake();

            AwakeUI();

            IMGUI_global.instance.users_ongui.AddElement(OnOnGui, this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            NUCLEOR.delegates.onLateUpdate -= OnLateUpdate;
            NUCLEOR.delegates.onLateUpdate += OnLateUpdate;

            UsageManager.ToggleUser(this, true, UsageGroups.TrueMouse, UsageGroups.Keyboard, UsageGroups.BlockPlayers, UsageGroups.Typing);

            flag_stdout.Update(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            NUCLEOR.delegates.onLateUpdate -= OnLateUpdate;
            UsageManager.RemoveUser(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            StartUI();

            NUCLEOR.delegates.getInputs += OnGetInputs;
            NUCLEOR.delegates.onPlayerInputs += OnUpdate;

            fullscreen.AddListener(value => flag_stdout.Update(true));

            shell = Util.InstantiateOrCreate<Shell>(transform);

            NUCLEOR.instance.subScheduler.AddRoutine(Util.EWaitForSeconds(.5f, false, () => sgui_toggle_window.Update(false)));
            SguiGlobal.instance.button_terminal.instances.AddElement(this);

            LogManager.ReadLastLogs(250, AddLine_log);
            LogManager.ToggleListener(true, AddLine_log);
        }

        //--------------------------------------------------------------------------------------------------------------

        public override void OnStateMachine(in AnimatorStateInfo stateInfo, in int layerIndex, in bool onEnter)
        {
            base.OnStateMachine(stateInfo, layerIndex, onEnter);
            if (onEnter)
                switch (state_base)
                {
                    case BaseStates.toActive:
                    case BaseStates.Active:
                        ForceSelectStdin();
                        break;
                }
        }

        void ITerminal.SetStdin(string text)
        {
            NUCLEOR.instance.subScheduler.AddRoutine(Util.EWaitForFrames(1, () =>
            {
                stdin_save = input_stdin.input_field.text = text;
                flag_stdin.Update(true);
            }));
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
            SguiGlobal.instance?.button_terminal?.instances.AddElement(this);
            LogManager.ToggleListener(false, AddLine_log);
        }
    }
}