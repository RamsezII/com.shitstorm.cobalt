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

        //--------------------------------------------------------------------------------------------------------------

        public CommandLine(in string text, in CMD_SIGNAL signal, in int cursor_i = default, in int cpl_index = default)
        {
            this.text = text ?? string.Empty;
            this.signal = signal;
            this.cursor_i = cursor_i;
            this.cpl_index = cpl_index;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <returns> stop parsing if false </returns>
        public bool ReadArgument(out string argument, out bool isNotEmpty, in IEnumerable<string> completions_candidates = null)
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

            // try completion
            if (signal >= CMD_SIGNAL.TAB && cursor_i >= start_i && cursor_i <= read_i)
            {
                if (completions_candidates != null)
                {
                    if (signal == CMD_SIGNAL.TAB)
                        ComputeCompletion_tab(argument, completions_candidates);
                    else if (signal >= CMD_SIGNAL.ALT_UP)
                        ComputeCompletion_alt(argument, completions_candidates);
                }
                return false;
            }
            else
                return true;
        }

        public bool TryReadPipe() => Util_ark.TryReadPipe(text, ref read_i);
    }
}