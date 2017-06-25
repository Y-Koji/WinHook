using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using HookLib;
using HookLib.Data.Mouse;

namespace MouseExender
{
    /*
     * MouseExtender
     *   マウス中クリック: マイクラ最小化（ｺｺ重要!
     *   マウス中クリック + 下にスクロール: 機能の有効化
     *   マウス中クリック + 上にスクロール: 機能の無効化
     * 
     * 機能
     *   下スクロールで左クリック(遅)
     *   上スクロールで左クリック(早)
     * 
     * ** 有用性 **
     * 授業中カチカチ鳴らさないでゲームができる（）
     * 
     */

    class Program
    {
        [DllImport("User32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_SHOWMINNOACTIVE = 7;

        [STAThread]
        static void Main(string[] args)
        {
            bool set = false;
            bool clicked = false;

            MouseHook.Set();

            MouseHook.WheelDown += () =>
            {
                foreach (var p in Process.GetProcessesByName("java"))
                    ShowWindow(p.MainWindowHandle, SW_SHOWMINNOACTIVE);
            };

            MouseHook.DownScroll += () =>
            {
                if (MouseHook.MiddleButton)
                {
                    set = true;

                    Debug.WriteLine("SET MouseExtender");
                }
                else if (set)
                {
                    if (clicked)
                        MouseHook.Rize(MouseEvent.LEFTUP);
                    else
                        MouseHook.Rize(MouseEvent.LEFTDOWN);

                    clicked = !clicked;
                }
            };

            MouseHook.UpScroll += () =>
            {
                if (MouseHook.MiddleButton)
                {
                    set = false;

                    Debug.WriteLine("UNSET MouseExtender");
                }
                else if (set)
                {
                    MouseHook.Rize(MouseEvent.LEFTDOWN);
                    MouseHook.Rize(MouseEvent.LEFTUP);
                }
            };


            while (true)
                Task.Delay(60000).Wait();
        }
    }
}
