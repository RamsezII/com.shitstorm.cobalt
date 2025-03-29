using _UTIL_;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        [Serializable]
        internal readonly struct LogInfos
        {
            public readonly string message;
            public readonly string stackTrace;
            public readonly LogType type;

            //--------------------------------------------------------------------------------------------------------------

            public LogInfos(in string item1, in string item2, in LogType item3)
            {
                message = item1;
                stackTrace = item2;
                type = item3;
            }

            public static implicit operator (string, string, LogType)(in LogInfos value) => (value.message, value.stackTrace, value.type);

            public static implicit operator LogInfos(in (string, string, LogType) value) => new(value.Item1, value.Item2, value.Item3);
        }

        static readonly List<LogInfos> logs_queue = new();

        //--------------------------------------------------------------------------------------------------------------

        static void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            message = message.TrimEnd('\n', '\r');
            LogInfos log = new(message, stackTrace, type);

            if (TryGetActiveTerminal(out Terminal terminal))
            {
                terminal.PullLogs();
                terminal.AddLog(log);
            }
            else
                lock (logs_queue)
                    logs_queue.Add(log);
        }

        //--------------------------------------------------------------------------------------------------------------

        void PullLogs()
        {
            lock (logs_queue)
                if (logs_queue.Count > 0)
                {
                    foreach (LogInfos log in logs_queue)
                        AddLog(log);
                    logs_queue.Clear();
                }
        }

        void AddLog(in LogInfos log)
        {
            switch (log.type)
            {
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    AddLine($"<color=red>{log.message}</color>");
                    break;
                case LogType.Warning:
                    AddLine($"<color=yellow>{log.message}</color>");
                    break;
                default:
                    AddLine(log.message);
                    break;
            }
        }
    }
}