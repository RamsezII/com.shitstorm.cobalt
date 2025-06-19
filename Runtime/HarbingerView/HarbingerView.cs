using _BOA_;
using _SGUI_;

namespace _COBALT_
{
    public sealed partial class HarbingerView : ShellView
    {
        public Shell shell;

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            shell = GetComponent<Shell>();
            base.Awake();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override char OnValidateInput(string text, int charIndex, char addedChar)
        {
            return base.OnValidateInput(text, charIndex, addedChar);
        }

        protected override void OnValueChanged(string value)
        {
            base.OnValueChanged(value);
        }
    }
}