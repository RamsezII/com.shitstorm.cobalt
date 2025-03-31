using System;
using UnityEngine;

namespace _COBALT_
{
    partial class Terminal
    {
        enum InputsEnum : byte
        {
            ctrl,
            shift,
            alt,
            tab,
            l_key,
            c_key,
            up,
            right,
            down,
            left,

            _last_,
        }

        [Flags]
        public enum InputsFlags : ushort
        {
            Ctrl = 1 << InputsEnum.ctrl,
            Shift = 1 << InputsEnum.shift,
            Alt = 1 << InputsEnum.alt,
            Tab = 1 << InputsEnum.tab,
            L_key = 1 << InputsEnum.l_key,
            C_key = 1 << InputsEnum.c_key,
            Up = 1 << InputsEnum.up,
            Right = 1 << InputsEnum.right,
            Down = 1 << InputsEnum.down,
            Left = 1 << InputsEnum.left,
        }

        [Header("~@ Inputs @~")]
        public InputsFlags inputs_hold;
        [HideInInspector] public InputsFlags inputs_down, inputs_up;
        [HideInInspector] public float scroll_y;

        //--------------------------------------------------------------------------------------------------------------

        public bool GetInputs_down(in InputsFlags inputs) => (inputs_down & inputs) != 0;
        public bool GetInputs_hold(in InputsFlags inputs) => (inputs_hold & inputs) != 0;
        public bool GetInputs_up(in InputsFlags inputs) => (inputs_up & inputs) != 0;

        void OnGetInputs()
        {
            InputsFlags inputs = 0;

            if (Application.isFocused)
                scroll_y = Input.mouseScrollDelta.y;
            else
                scroll_y = 0;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                inputs |= InputsFlags.Ctrl;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                inputs |= InputsFlags.Shift;
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                inputs |= InputsFlags.Alt;
            if (Input.GetKey(KeyCode.Tab))
                inputs |= InputsFlags.Tab;

            if (Input.GetKey(KeyCode.L))
                inputs |= InputsFlags.L_key;
            if (Input.GetKey(KeyCode.C))
                inputs |= InputsFlags.C_key;

            if (Input.GetKey(KeyCode.UpArrow))
                inputs |= InputsFlags.Up;
            if (Input.GetKey(KeyCode.RightArrow))
                inputs |= InputsFlags.Right;
            if (Input.GetKey(KeyCode.DownArrow))
                inputs |= InputsFlags.Down;
            if (Input.GetKey(KeyCode.LeftArrow))
                inputs |= InputsFlags.Left;

            inputs_down = inputs & ~inputs_hold;
            inputs_up = inputs_hold & ~inputs;
            inputs_hold = inputs;
        }
    }
}