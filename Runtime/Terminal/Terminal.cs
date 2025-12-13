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

        public readonly ValueHandler<KeyCode>
            flag_alt = new(),
            flag_nav_history = new();

        [SerializeField] string stdin_save;
        [SerializeField] int cpl_index;
        [SerializeField] int stdin_frame, tab_frame;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            OSView.instance.AddSoftwareButton<Terminal>(new("Cobalt terminal"));
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            base.OnAwake();

            AwakeUI();
            trad_title.SetTrad("COBALT");

            IMGUI_global.instance.inputs_users.AddElement(OnOnGuiInputs);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            NUCLEOR.delegates.LateUpdate -= OnLateUpdate;
            NUCLEOR.delegates.LateUpdate += OnLateUpdate;

            UsageManager.ToggleUser(this, true, UsageGroups.TrueMouse, UsageGroups.Keyboard, UsageGroups.BlockPlayer, UsageGroups.Typing);

            flag_stdout.Value = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            NUCLEOR.delegates.LateUpdate -= OnLateUpdate;
            UsageManager.RemoveUser(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            StartUI();

            NUCLEOR.delegates.Update_GettInputs += OnGetInputs;
            NUCLEOR.delegates.Update_OnPlayerInputs += OnUpdate;

            fullscreen.AddListener(value => flag_stdout.Value = true);

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
                        flag_stdout.Value = true;
                        ForceSelectStdin();
                        break;
                }
        }

        void ITerminal.SetStdin(string text)
        {
            NUCLEOR.instance.sequencer_parallel.AddRoutine(Util.EWaitForFrames(1, () =>
            {
                stdin_save = input_stdin.input_field.text = text;
                flag_stdin.Value = true;
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

            IMGUI_global.instance.inputs_users.RemoveElement(OnOnGuiInputs);
            LogManager.ToggleListener(false, AddLine_log);
        }

        protected override void OnDestroy()
        {
            NUCLEOR.delegates.Update_GettInputs -= OnGetInputs;
            NUCLEOR.delegates.Update_OnPlayerInputs -= OnUpdate;

            base.OnDestroy();

            if (this == instance_last)
                instance_last = null;
        }
    }
}