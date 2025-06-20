using _ARK_;

namespace _COBALT_
{
    partial class HarbingerView
    {
        void OnRefreshStdout() => Util.AddAction(ref NUCLEOR.delegates.onLateUpdate, RefreshStdout);
        void RefreshStdout()
        {
            std_out.inputfield.text = shell.stdout_text;
            std_out.lint.text = shell.stdout_lint;
        }
    }
}