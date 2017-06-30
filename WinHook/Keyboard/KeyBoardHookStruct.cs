using System;
using System.Runtime.InteropServices;

namespace WinHook.Keyboard
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyBoardHookStruct
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }
}