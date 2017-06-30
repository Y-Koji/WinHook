using System.Runtime.InteropServices;

namespace WinHook.Mouse
{
    /// <summary>
    /// マウスの座標構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;

    }
}
