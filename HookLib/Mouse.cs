using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HookLib
{
    using DataType = Data.Mouse;

    public static class MouseHook
    {
        private delegate IntPtr HookProcedureDelegate(int nCode, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// マウスで発生するすべてのイベント
        /// </summary>
        public static event Action<DataType.POINT, Enum> MouseEvent;
        /// <summary>
        /// マウスで上にスクロールをした場合に発生するイベント
        /// </summary>
        public static event Action UpScroll;
        /// <summary>
        /// マウスで下にスクロールをした場合に発生するイベント
        /// </summary>
        public static event Action DownScroll;
        /// <summary>
        /// マウスの任意のボタンが押下された場合に発生するイベント
        /// </summary>
        public static event Action<DataType.POINT, DataType.Click> MouseDown;
        /// <summary>
        /// マウスで任意のボタンが離された場合に発生するイベント
        /// </summary>
        public static event Action<DataType.POINT, DataType.Click> MouseUp;
        /// <summary>
        /// マウスホイールが離された場合に発生するイベント
        /// </summary>
        public static event Action WheelUp;
        /// <summary>
        /// マウスホイールが押下された場合に発生するイベント
        /// </summary>
        public static event Action WheelDown;
        /// <summary>
        /// クリック動作を行った場合に発生するイベント
        /// </summary>
        public static event Action<DataType.Click> Click;
        /// <summary>
        /// マウスが移動をした場合に発生するイベント
        /// </summary>
        public static event Action<DataType.POINT> Move;
        /// <summary>
        /// マウスのイベントを破棄する/破棄しないを決定 この設定はWindowsシステム全てに作用します
        /// </summary>
        public static event Func<DataType.POINT, Enum, bool> Filter;

        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProcedureDelegate lpfn, IntPtr hInstance, int ThreadId);
        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(IntPtr idHook);
        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(String lpModuleName);
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public DataType.POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private static IntPtr hHook = IntPtr.Zero;

        private enum HookType
        {
            WH_MOUSE = 7, WH_MOUSE_LL = 14,
        }

        /// <summary>
        /// グローバルフックを開始
        /// </summary>
        /// <returns></returns>
        public static bool Set()
        {
            if (hHook == IntPtr.Zero)
            {
                HookProcedureDelegate HookProc = MouseHookProc;
                GCHandle.Alloc(HookProc, GCHandleType.Normal);

                using (Process p = Process.GetCurrentProcess())
                using (ProcessModule m = p.MainModule)
                    hHook = SetWindowsHookEx((int)HookType.WH_MOUSE_LL, HookProc, m.BaseAddress, 0);

                if (hHook == IntPtr.Zero)
                    return false;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// マウスフックを停止
        /// </summary>
        /// <returns></returns>
        public static bool Remove()
        {
            if (UnhookWindowsHookEx(hHook))
            {
                hHook = IntPtr.Zero;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// マウスイベントをシステムに対して発行します
        /// </summary>
        /// <param name="e"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="wheel_up"></param>
        public static void Rize(DataType.MouseEvent e, int dx = 0, int dy = 0, bool wheel_up = true)
        {
            if (DataType.MouseEvent.WHEEL != e)
                mouse_event((int)e, dx, dy, 0, 0);
            else
                mouse_event((int)e, 0, 0, wheel_up ? 1 : -1, 0);
        }

        private static IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseHookStruct mouseStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            #region グローバルフィルタ
            if (Filter != null)
            {
                /* フィルタの結果falseならイベントデータを破棄する。 */
                if (mouseStruct.mouseData == (uint)DataType.Scroll.Down || mouseStruct.mouseData == (uint)DataType.Scroll.Up)
                {
                    if (Filter(mouseStruct.pt, (DataType.Scroll)mouseStruct.mouseData) == false)
                        return (IntPtr)1;
                }
                else
                {
                    if (Filter(mouseStruct.pt, (DataType.Click)wParam) == false)
                        return (IntPtr)1;
                }
            }
            #endregion

            #region 各種コールバック呼び出し
            if (mouseStruct.mouseData == (uint)DataType.Scroll.Up)
            {
                if (UpScroll != null)
                    UpScroll();
            }
            else if (mouseStruct.mouseData == (uint)DataType.Scroll.Down)
            {
                if (DownScroll != null)
                    DownScroll();
            }
            else if ((DataType.Click)wParam != DataType.Click.None)
            {
                if (Click != null)
                    Click((DataType.Click)wParam);

                switch ((DataType.Click)wParam)
                {
                    case DataType.Click.LeftDown:
                        if (MouseDown != null)
                            MouseDown(mouseStruct.pt, DataType.Click.LeftDown);
                        break;
                    case DataType.Click.LeftUp:
                        if (MouseUp != null)
                            MouseUp(mouseStruct.pt, DataType.Click.LeftUp);
                        break;
                    case DataType.Click.RightDown:
                        if (MouseDown != null)
                            MouseDown(mouseStruct.pt, DataType.Click.RightDown);
                        break;
                    case DataType.Click.RightUp:
                        if (MouseUp != null)
                            MouseUp(mouseStruct.pt, DataType.Click.RightUp);
                        break;
                    case DataType.Click.WheelDown:
                        _middleButton = true;
                        if (WheelDown != null)
                            WheelDown();
                        break;
                    case DataType.Click.WheelUp:
                        _middleButton = false;
                        if (WheelUp != null)
                            WheelUp();
                        break;
                }
            }
            else
            {
                if (Move != null)
                    Move(mouseStruct.pt);
            }

            if (MouseEvent != null)
            {
                if (mouseStruct.mouseData == (uint)DataType.Scroll.Down || mouseStruct.mouseData == (uint)DataType.Scroll.Up)
                    MouseEvent(mouseStruct.pt, (DataType.Scroll)mouseStruct.mouseData);
                else
                    MouseEvent(mouseStruct.pt, (DataType.Click)wParam);
            }
            #endregion

            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        private static bool _middleButton = false;
        public static bool MiddleButton { get { return _middleButton; } }
    }
}
