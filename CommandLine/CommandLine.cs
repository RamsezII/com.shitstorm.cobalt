using _ARK_;

namespace _COBALT_
{
    public class CommandLine
    {
        public string text;
        public CMD_SIGNAL signal;
        public int read_i, start_i, arg_i;

        //--------------------------------------------------------------------------------------------------------------

        public CommandLine(in string text, in CMD_SIGNAL signal)
        {
            this.signal = signal;
            this.text = text;
        }

        //--------------------------------------------------------------------------------------------------------------

        public bool TryReadArgument(out string argument)
        {
            Util_ark.SkipCharactersUntil(text, ref read_i, true);
            start_i = read_i;
            Util_ark.SkipCharactersUntil(text, ref read_i, false);

            if (start_i < read_i)
            {
                argument = text[start_i..read_i];
                return true;
            }

            argument = string.Empty;
            return false;
        }
    }
}