using _ARK_;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        void AddLine_log(LogManager.LogInfos log)
        {
            switch (log.type)
            {
                case LogType.Exception:
                case LogType.Assert:
                    AddLine(log.message, $"<color=red>{log.message}</color>");
                    break;
                case LogType.Error:
                    AddLine(log.message, $"<color=orange>{log.message}</color>");
                    break;
                case LogType.Warning:
                    AddLine(log.message, $"<color=yellow>{log.message}</color>");
                    break;
                default:
                    AddLine(log.message, log.message);
                    break;
            }
        }
    }
}