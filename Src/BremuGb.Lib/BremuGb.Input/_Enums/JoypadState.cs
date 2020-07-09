using System;

namespace BremuGb.Input
{
    [Flags]
    public enum JoypadState
    {
        None = 0,
        A = 1,
        B = 2,
        Select = 4,
        Start = 8,
        Up = 16,
        Down = 32,
        Left = 64,
        Right = 128,
    }
}
