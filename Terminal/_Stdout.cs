using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        readonly Queue<object> lines = new();

        [SerializeField, TextArea(1, 10)] string stdout;

        const byte max_lines = 250;

        //--------------------------------------------------------------------------------------------------------------

        public void AddLine(in object line)
        {
            lock (lines)
            {
                while (lines.Count >= max_lines)
                    lines.Dequeue();
                lines.Enqueue(line);
                flag_stdout.Update(true);
            }
        }

        public void RefreshStdout()
        {
            StringBuilder sb = new();
            lock (lines)
            {
                foreach (object line in lines)
                    sb.AppendLine(line.ToString());
            }
            stdout = sb.TroncatedForLog();

            input_stdout.input_field.text = stdout;
            input_stdout.AutoSize(true);

            RefreshStdin();
            flag_clampbottom.Update(true);
        }

        public void ClearStdout()
        {
            scrollview.verticalNormalizedPosition = 0;
        }
    }
}