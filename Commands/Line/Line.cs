namespace _COBALT_
{
    partial class Command
    {
        public sealed partial class Line
        {
            public static readonly Line EMPTY_EXE = new(default, CMD_SIGNALS.EXEC);

            public string text;
            public int cpl_index;
            public CMD_SIGNALS signal;
            public int cursor_i, read_i, start_i, arg_i = -1, cpl_start_i;
            public string arg_last;

            public bool IsCplThis => signal >= CMD_SIGNALS.TAB && cursor_i >= start_i && cursor_i <= read_i;
            public bool IsCplOverboard => signal >= CMD_SIGNALS.TAB && read_i > cursor_i;

            //--------------------------------------------------------------------------------------------------------------

            public Line(in string text, in CMD_SIGNALS signal, in int cursor_i = default, in int cpl_index = default)
            {
                this.text = text ?? string.Empty;
                this.signal = signal;
                this.cursor_i = cursor_i;
                this.cpl_index = cpl_index;
            }
        }
    }
}