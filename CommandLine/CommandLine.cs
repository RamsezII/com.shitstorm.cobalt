using _ARK_;
using System.Collections.Generic;

namespace _COBALT_
{
    public sealed partial class CommandLine
    {
        public static readonly CommandLine EMPTY_EXE = new(default, CMD_SIGNALS.EXEC);

        public string text;
        public int cpl_index;
        public CMD_SIGNALS signal;
        public int cursor_i, read_i, start_i, next_i, arg_i = -1, cpl_start_i;
        public bool IsCplThis => signal >= CMD_SIGNALS.TAB && cursor_i >= start_i && cursor_i <= read_i;
        public bool IsCplOverboard => signal >= CMD_SIGNALS.TAB && read_i > cursor_i;

        //--------------------------------------------------------------------------------------------------------------

        public CommandLine(in string text, in CMD_SIGNALS signal, in int cursor_i = default, in int cpl_index = default)
        {
            this.text = text ?? string.Empty;
            this.signal = signal;
            this.cursor_i = cursor_i;
            this.cpl_index = cpl_index;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <returns> stop parsing if false </returns>
        public bool TryReadArgument(out string argument, in IEnumerable<string> completions_candidates = null)
        {
            Util_ark.SkipCharactersUntil(text, ref read_i, true);
            start_i = read_i;
            Util_ark.SkipCharactersUntil(text, ref read_i, false);
            next_i = read_i;
            Util_ark.SkipCharactersUntil(text, ref next_i, true);

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

        public bool TryReadCommand(in Command parent, out List<KeyValuePair<string, Command>> path)
        {
            path = new();
            return TryReadCommand_ref(parent, path);
        }

        bool TryReadCommand_ref(in Command parent, in List<KeyValuePair<string, Command>> path)
        {
            if (TryReadArgument(out string cmd_name, parent.ECommands_keys))
                if (parent._commands.TryGetValue(cmd_name, out Command intermediate))
                {
                    path.Add(new(cmd_name, intermediate));
                    TryReadCommand_ref(intermediate, path);
                }
                else
                    read_i = start_i;
            return path.Count > 0;
        }
    }
}