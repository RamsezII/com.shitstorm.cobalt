using _ARK_;
using _UTIL_;

namespace _COBALT_
{
    partial class Terminal
    {
        public readonly OnValue<bool>
            flag_realtime = new(),
            flag_stdout = new(),
            flag_stdin = new(),
            flag_clampbottom = new();

        //--------------------------------------------------------------------------------------------------------------

        void CheckFlags()
        {
            if (flag_realtime.PullValue)
                RefreshRealtime();

            if (flag_stdout.PullValue)
                RefreshStdout();

            if (flag_alt.Value != default)
                OnAltKey();

            if (flag_stdin.PullValue)
                RefreshStdin();

            if (flag_clampbottom.PullValue)
                NUCLEOR.delegates.onEndOfFrame_once += ClampBottom;
        }
    }
}