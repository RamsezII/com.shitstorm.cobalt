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
        [SerializeField] int frame_tab;

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
            shell.on_stdout += OnRefreshStdout;
        }
    }
}