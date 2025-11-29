using _ARK_;
using _COBRA_;
using _SGUI_;
using UnityEngine;

namespace _COBALT_
{
    public sealed partial class HarbingerView : ShellView
    {
        public SguiTerminal terminal;
        public _BOA_.Shell shell;
        public LintTheme lint_theme;

        [SerializeField] _BOA_.Contract.Status last_status;
        [SerializeField] float stdin_h, stdout_h;
        [SerializeField] bool flag_history;

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            terminal = GetComponentInParent<SguiTerminal>();
            shell = GetComponent<_BOA_.Shell>();
            base.Awake();
            shell.change_stdin += value => stdin_field.text = value;
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            shell.on_stdout += RefreshStdout;
            ResetStdin();
            NUCLEOR.delegates.LateUpdate += OnLateUpdate;
            RefreshStdout();
            stdin_field.text = "> ";
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            NUCLEOR.delegates.LateUpdate -= OnLateUpdate;
            base.OnDestroy();
        }
    }
}