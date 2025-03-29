using System;
using System.Collections.Generic;

namespace _COBALT_
{
    public partial class Command
    {
        public readonly static Command root_instance = new();

        public readonly Dictionary<string, Command> commands = new(StringComparer.OrdinalIgnoreCase);

        //--------------------------------------------------------------------------------------------------------------


    }
}