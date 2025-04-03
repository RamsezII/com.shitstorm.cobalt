using _ARK_;
using System.Collections.Generic;

namespace _COBALT_
{
    public sealed partial class CommandLine
    {
        public static readonly CommandLine EMPTY = new(default, default);

        public string text;
        public int cpl_index;
        public CMD_SIGNAL signal;
        public int cursor_i, read_i, start_i, next_i, arg_i = -1;
        public bool IsCplThis => signal == CMD_SIGNAL.TAB && cursor_i >= start_i && cursor_i <= read_i;
        public bool IsCplOverboard => signal == CMD_SIGNAL.TAB && cursor_i < read_i;

        //--------------------------------------------------------------------------------------------------------------

        public CommandLine(in string text, in CMD_SIGNAL signal, in int cursor_i = default, in int cpl_index = default)
        {
            this.text = text ?? string.Empty;
            this.signal = signal;
            this.cursor_i = cursor_i;
            this.cpl_index = cpl_index;
        }

        //--------------------------------------------------------------------------------------------------------------

        public bool TryReadArgument(out string argument, out bool isNotEmpty, IEnumerable<string> completions_candidates)
        {
            Util_ark.SkipCharactersUntil(text, ref read_i, true);
            start_i = read_i;
            Util_ark.SkipCharactersUntil(text, ref read_i, false);
            next_i = read_i;
            Util_ark.SkipCharactersUntil(text, ref next_i, true);

            if (start_i < read_i)
            {
                argument = text[start_i..read_i];
                ++arg_i;

                isNotEmpty = true;
            }
            else
            {
                argument = string.Empty;
                isNotEmpty = false;
            }

            if (IsCplThis)
                ComputeCompletion_tab(argument, completions_candidates);

            return isNotEmpty;
        }
    }
}