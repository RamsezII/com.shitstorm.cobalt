using System;
using System.Collections.Generic;

namespace _COBALT_
{
    public partial class Command
    {
        public readonly static Command root_instance = new();

        public readonly Dictionary<string, Command> commands = new(StringComparer.OrdinalIgnoreCase);

        public string prefixe;

        public Action action;
        public IEnumerator<float> routine;

        //--------------------------------------------------------------------------------------------------------------

        public Command(in string prefixe = null, in Action action = null, in IEnumerator<float> routine = null)
        {
            if (prefixe == null)
                this.prefixe = GetType().FullName;
            else
                this.prefixe = prefixe;

            this.action = action;
            this.routine = routine;
        }
    }
}