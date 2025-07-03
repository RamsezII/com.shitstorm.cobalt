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
        public LintTheme lint_theme;

        [SerializeField] Contract.Status last_status;
        [SerializeField] float stdin_h, stdout_h;
        [SerializeField] bool flag_history;

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            terminal = GetComponentInParent<SguiTerminal>();
            shell = GetComponent<Shell>();
            base.Awake();
            shell.change_stdin += value => stdin_field.text = value;
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            shell.on_stdout += RefreshStdout;
            ResetStdin();
            NUCLEOR.delegates.onLateUpdate += OnLateUpdate;
            RefreshStdout();
            stdin_field.text = "> ";
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            NUCLEOR.delegates.onLateUpdate -= OnLateUpdate;
            base.OnDestroy();
        }
    }
}