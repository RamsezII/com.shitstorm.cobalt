using _ARK_;
using _BOA_;
using _SGUI_;
using UnityEngine;

namespace _COBALT_
{
    public sealed partial class HarbingerView : ShellView
    {
        public SguiTerminal terminal;
        public Shell shell;
        [SerializeField] string stdin_save;
        [SerializeField] int last_input, last_tab;

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            terminal = GetComponentInParent<SguiTerminal>();
            shell = GetComponent<Shell>();
            base.Awake();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            shell.on_stdout += RefreshStdout;
            ResetStdin();
            NUCLEOR.delegates.onLateUpdate += OnLateUpdate;
            RefreshStdout();
            stdin_field.inputfield.text = "> ";
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            NUCLEOR.delegates.onLateUpdate -= OnLateUpdate;
            base.OnDestroy();
        }
    }
}