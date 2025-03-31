using _UTIL_;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        [SerializeField, TextArea(1, 10)] string stdout;

        const byte max_lines = 100;
        readonly Queue<object> lines = new();
        public readonly OnValue<bool> refresh_stdout = new();

        //--------------------------------------------------------------------------------------------------------------

        public void AddLine(in object line)
        {
            lock (lines)
            {
                while (lines.Count >= max_lines)
                    lines.Dequeue();
                lines.Enqueue(line);
                refresh_stdout.Update(true);
            }
        }

        public void RefreshStdout()
        {
            refresh_stdout.Update(false);

            StringBuilder sb = new();
            lock (lines)
            {
                foreach (object line in lines)
                    sb.AppendLine(line.ToString());
            }
            stdout = sb.TroncatedForLog();

            input_stdout.input_field.text = stdout;
            input_stdout.AutoSize();

            RefreshStdin();
        }
    }
}