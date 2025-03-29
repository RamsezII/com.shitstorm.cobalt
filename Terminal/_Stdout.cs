using _ARK_;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        readonly Queue<object> lines = new();
        [SerializeField, TextArea(1, 10)] string stdout;
        const byte maxLines = 100;

        //--------------------------------------------------------------------------------------------------------------

        public void AddLine(in object line)
        {
            lock (lines)
            {
                while (lines.Count >= maxLines)
                    lines.Dequeue();

                lines.Enqueue(line);

                lock (NUCLEOR.instance.mainThreadLock)
                    NUCLEOR.delegates.onEndOfFrame += RefreshLines;
            }
        }

        public void RefreshLines()
        {
            StringBuilder sb = new();
            lock (lines)
            {
                foreach (object line in lines)
                    sb.AppendLine(line.ToString());
            }
            stdout = sb.TroncatedForLog();
        }
    }
}