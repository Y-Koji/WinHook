using System;
using System.Runtime.InteropServices;

namespace WinHook.Mouse
{
    [StructLayout(LayoutKind.Sequential)]
    public class MouseHookStruct
    {
        public Point pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;

    }
}
