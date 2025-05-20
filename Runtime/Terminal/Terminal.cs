using _ARK_;
using _COBRA_;
using _SGUI_;
using _UTIL_;
using UnityEngine;

namespace _COBALT_
{
    public partial class Terminal : SguiWindow1, ITerminal
    {
        public static Terminal instance_last;

        public Shell shell;
        Shell ITerminal.GetShell => shell;
        void ITerminal.Exit() => Oblivionize();
        void ITerminal.ClearLines() => lines.Clear();

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
            InitSoftwareButton();
            IMGUI_global.instance.users_ongui.AddElement(OnOnGui_static, new());
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            SguiGlobal.instance.button_terminal.instances.AddElement(this);

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
                        flag_stdout.Update(true);
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

        protected override void OnToggleWindow(in bool toggle)
        {
            base.OnToggleWindow(toggle);
            if (toggle)
                instance_last = this;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnOblivion()
        {
            base.OnOblivion();

            IMGUI_global.instance.users_ongui.RemoveElement(this);
            SguiGlobal.instance?.button_terminal?.instances.RemoveElement(this);
            LogManager.ToggleListener(false, AddLine_log);
        }

        protected override void OnDestroy()
        {
            NUCLEOR.delegates.getInputs -= OnGetInputs;
            NUCLEOR.delegates.onPlayerInputs -= OnUpdate;

            base.OnDestroy();

            if (this == instance_last)
                instance_last = null;
        }
    }
}