namespace WinHook.Mouse
{
    public enum MouseEvent
    {
        RIGHTDOWN = 0x8, RIGHTUP = 0x10,
        LEFTDOWN = 0x2, LEFTUP = 0x4,
        MIDDLEDOWN = 0x20, MIDDLEUP = 0x40,
        MOVE = 0x1,
        WHEEL = 0x800, HWHEEL = 0x1000,
        XDOWN = 0x80, XUP = 0x100,
    }
}
