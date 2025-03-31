using System.Collections.Generic;

namespace _COBALT_
{
    public class CommandLine
    {
        public readonly string text;
        public readonly List<string> arguments = new();
        public int read_start, read_cursor;

        //--------------------------------------------------------------------------------------------------------------

        public CommandLine(in string text)
        {
            this.text = text;
        }

        //--------------------------------------------------------------------------------------------------------------


    }
}