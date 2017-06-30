using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinHook.Keyboard;
using WinHook.Mouse;

namespace WinHook
{
    /// <summary>フック ベースクラス</summary>
    public abstract class Hook : IHook
    {
        protected delegate IntPtr WinProc(int nCode, IntPtr wParam, IntPtr lParam);

        protected enum HookType
        {
            WH_MOUSE = 7, WH_MOUSE_LL = 14,
            WH_KEYBOARD_LL = 13,
        }

        protected IntPtr _hHook;
        protected HookChain _chain;

        protected HookType Type { get; private set; }

        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr SetWindowsHookEx(HookType idHook, WinProc lpfn, IntPtr hInstance, int ThreadId);
        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern bool UnhookWindowsHookEx(IntPtr idHook);
        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern IntPtr GetModuleHandle(String lpModuleName);
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        abstract protected IntPtr Procedure(int nCode, IntPtr wParam, IntPtr lParam);

        public HookChain Set()
        {
            if (IntPtr.Zero == _hHook)
            {
                _chain = new HookChain(this);

                WinProc proc = Procedure;
                GCHandle.Alloc(proc, GCHandleType.Normal);
                using (Process p = Process.GetCurrentProcess())
                using (ProcessModule m = p.MainModule)
                {
                    _hHook = SetWindowsHookEx(Type, proc, m.BaseAddress, 0);
                }
            }

            return _chain;
        }

        public void UnSet()
        {
            if (UnhookWindowsHookEx(_hHook))
            {
                _hHook = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            UnSet();
        }

        /// <summary>
        /// マウスのグローバルフックインスタンスを取得します。
        /// このフックでは「Click」「Scroll」「MouseMove」オブジェクトが流れてきます。
        /// </summary>
        public static IHook Mouse { get; private set; } = new MouseHook { Type = HookType.WH_MOUSE_LL };

        /// <summary>
        /// キーボードのグローバルフックインスタンスを取得します。
        /// このフックでは「KeyCode」「KeyState」「char」オブジェクトが流れてきます。
        /// </summary>
        public static IHook Keyboard { get; private set; } = new KeyboardHook { Type = HookType.WH_KEYBOARD_LL };
    }
}
