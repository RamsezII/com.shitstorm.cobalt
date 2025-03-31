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
        public readonly Func<CommandLine, CMD_STATUS> action;
        public readonly Func<Executor, IEnumerator<CMD_STATUS>> routine;

        //--------------------------------------------------------------------------------------------------------------

        public Command(in bool hide_stdout, in Traductions manual, in Func<CommandLine, CMD_STATUS> action) : this(hide_stdout, manual)
        {
            this.action = action;
        }

        public Command(in bool hide_stdout, in Traductions manual, in Func<Executor, IEnumerator<CMD_STATUS>> routine) : this(hide_stdout, manual)
        {
            this.routine = routine;
        }

        public Command(in bool hide_stdout, in Traductions manual)
        {
            this.hide_stdout = hide_stdout;
            this.manual = manual;
        }
    }
}