using _BOA_;
using _SGUI_;
using System;
using UnityEngine;

namespace _COBALT_
{
    public sealed partial class HarbingerView : ShellView
    {
        public SguiTerminal terminal;
        public Shell shell;
        [SerializeField] string stdin_save;

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            terminal = GetComponentInParent<SguiTerminal>();
            shell = GetComponent<Shell>();
            base.Awake();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override char OnValidateInput(string text, int charIndex, char addedChar)
        {
            if (shell.current_status.state != Contract.Status.States.WAIT_FOR_STDIN)
            {
                std_in.ResetText();
                return '\0';
            }

            try
            {
                switch (addedChar)
                {
                    case '\t':
                        OnTab(text, charIndex);
                        return '\0';

                    case '\n':
                        OnSubmit(text);
                        return '\0';
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return '\0';
            }
            return addedChar;
        }

        protected override void OnValueChanged(string value)
        {
            switch (shell.current_status.state)
            {
                case Contract.Status.States.WAIT_FOR_STDIN:
                    stdin_save = value;
                    break;

                case Contract.Status.States.BLOCKING:
                default:
                    stdin_save = string.Empty;
                    if (!string.IsNullOrEmpty(std_in.inputfield.text))
                        std_in.ResetText();
                    break;
            }
        }
    }
}