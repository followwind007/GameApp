using System;
using UnityEngine;

namespace GameApp.DebugConsole
{
    [Serializable]
    public struct LogMessage
    {
        public string condition;
        public string stackTrace;
        public LogType type;
    }
}