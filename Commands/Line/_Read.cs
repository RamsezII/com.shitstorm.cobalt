using _ARK_;
using System.Collections.Generic;

namespace _COBALT_
{
    partial class Command
    {
        partial class Line
        {
            public bool TryReadArgument(out string argument, in IEnumerable<string> completions_candidates = null)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    argument = string.Empty;
                    return false;
                }

                Util_ark.SkipCharactersUntil(text, ref read_i, true, false, Util_ark.CHAR_SPACE);

                if (read_i > 0 && text[read_i - 1] != Util_ark.CHAR_SPACE)
                {
                    argument = string.Empty;
                    return false;
                }

                start_i = read_i;
                Util_ark.SkipCharactersUntil(text, ref read_i, true, true, Util_ark.CHAR_SPACE, Util_ark.CHAR_PIPE, Util_ark.CHAR_CHAIN);

                bool isNotEmpty = false;

                if (start_i < read_i)
                {
                    argument = text[start_i..read_i];
                    ++arg_i;

                    isNotEmpty = true;
                }
                else
                    argument = string.Empty;

                // try completion
                if (completions_candidates != null)
                    if (IsCplThis)
                    {
                        cpl_start_i = read_i;
                        if (signal == CMD_SIGNALS.TAB)
                            ComputeCompletion_tab(argument, completions_candidates);
                        else if (signal >= CMD_SIGNALS.ALT_UP)
                            ComputeCompletion_alt(argument, completions_candidates);
                    }

                return isNotEmpty;
            }

            public bool TryReadPipe() => Util_ark.TryReadPipe(text, ref read_i);
        }
    }
}