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

        static readonly ThreadSafe<List<LogInfos>> logs_queue = new();

        //--------------------------------------------------------------------------------------------------------------

        static void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            message = message.TrimEnd('\n', '\r');
            logs_queue.Value.Add((message, stackTrace, type));
        }

        //--------------------------------------------------------------------------------------------------------------

        void PullLogs()
        {

        }
    }
}