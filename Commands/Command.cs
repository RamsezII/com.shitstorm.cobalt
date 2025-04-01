using _UTIL_;
using System;
using System.Collections.Generic;

namespace _COBALT_
{
    public sealed partial class Command
    {
        public readonly Dictionary<string, Command> commands = new(StringComparer.OrdinalIgnoreCase);

        //string prefixe = $"{"user".SetColor("#73CC26")}:{"~".SetColor("#73B2D9")}$";
        public readonly bool hide_stdout;
        public readonly Traductions manual;
        public readonly Action<CommandLine> action;
        public readonly Func<Executor, IEnumerator<CMD_STATUS>> routine;

        //--------------------------------------------------------------------------------------------------------------

        public Command(in bool hide_stdout = default, in Traductions manual = default, in Action<CommandLine> action = default, in Func<Executor, IEnumerator<CMD_STATUS>> routine = default)
        {
            this.hide_stdout = hide_stdout;
            this.manual = manual;
            this.action = action;
            this.routine = routine;
        }
    }
}