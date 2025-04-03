using UnityEngine;

namespace _COBALT_
{
    public enum CMD_SIGNAL : byte
    {
        _NONE_,
        CHECK,
        ERR,
        EXEC,
        ALT_DOUBLE,
        TAB,
        ALT_UP,
        ALT_DOWN,
        ALT_LEFT,
        ALT_RIGHT,
    }

    public enum CMD_STATE : byte
    {
        DONE,
        BLOCKING,
        WAIT_FOR_STDIN,
        FULLSCREEN_w,
        FULLSCREEN_r,
    }

    public struct CMD_STATUS
    {
        public CMD_STATE state;
        public string prefixe;
        [Range(0, 1)] public float progress;
        public bool refresh_stdin;
        public object data;
    }
}