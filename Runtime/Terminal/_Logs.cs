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

        static readonly List<LogInfos> pending_logs = new();
        static readonly OnValue<Action<LogInfos>> onLog = new();

        //--------------------------------------------------------------------------------------------------------------

        static void InitLogs()
        {
            lock (onLog)
                lock (pending_logs)
                {
                    pending_logs.Clear();
                    onLog._value = log =>
                    {
                        lock (pending_logs)
                            pending_logs.Add(log);
                    };
                }
        }

        //--------------------------------------------------------------------------------------------------------------

        static void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
                message = message.TrimEnd('\n', '\r');

            LogInfos log = new(message, stackTrace, type);

            lock (onLog)
                onLog._value(log);
        }

        //--------------------------------------------------------------------------------------------------------------

        void AddLine_log(LogInfos log)
        {
            switch (log.type)
            {
                case LogType.Exception:
                case LogType.Assert:
                    AddLine($"<color=red>{log.message}</color>");
                    break;
                case LogType.Error:
                    AddLine($"<color=orange>{log.message}</color>");
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