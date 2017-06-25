using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HookLib
{
    using Data.Keyboard;

    public static class KeyboardHook
    {
        /// <summary>
        /// キーボード上で発生するすべてのイベント
        /// </summary>
        public static event Action<KeyHookData> KeyBoardEvent;
        /// <summary>
        /// キーが押下された場合に発生するイベント
        /// </summary>
        public static event Action<KeyHookData> KeyDown;
        /// <summary>
        /// キーが離された場合に発生するイベント
        /// </summary>
        public static event Action<KeyHookData> KeyUp;
        /// <summary>
        /// キーボードのイベントを破棄する/破棄しないを決定 この設定はWindowsシステム全てに作用します
        /// </summary>
        public static event Func<KeyHookData, bool> Filter;

        private delegate IntPtr HookProcedureDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProcedureDelegate lpfn, IntPtr hInstance, int ThreadId);
        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(IntPtr idHook);
        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetModuleHandle(String lpModuleName);

        private static IntPtr hHook = IntPtr.Zero;

        private struct HookType
        {
            public static int WH_HOOK_LL = 13;
        }

        /// <summary>
        /// グローバルフックを開始
        /// </summary>
        /// <returns></returns>
        public static bool Set()
        {
            if (hHook == IntPtr.Zero)
            {
                HookProcedureDelegate HookProc = KeyBoardHookProc;
                GCHandle.Alloc(HookProc, GCHandleType.Normal);

                using (Process p = Process.GetCurrentProcess())
                using (ProcessModule m = p.MainModule)
                {
                    hHook = SetWindowsHookEx(HookType.WH_HOOK_LL, HookProc, GetModuleHandle(m.ModuleName), 0);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// グローバルフックを停止
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
            {
                return false;
            }
        }

        private struct KeyState
        {
            public static bool Ctrl = false;
            public static bool Shift = false;
        }

        private static IntPtr KeyBoardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KeyBoardHookStruct KeyBoardStruct = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));

            #region KeyStateの更新
            if (KeyBoardStruct.vkCode == (int)HookKeyCodes.Shift)
            {
                switch ((int)wParam)
                {
                    case (int)HookKeyState.KeyDown:
                        KeyState.Shift = true;
                        break;
                    case (int)HookKeyState.KeyUp:
                        KeyState.Shift = false;
                        break;
                }
            }

            if (KeyBoardStruct.vkCode == (int)HookKeyCodes.Ctrl)
            {
                switch ((int)wParam)
                {
                    case (int)HookKeyState.KeyDown:
                        KeyState.Ctrl = true;
                        break;
                    case (int)HookKeyState.KeyUp:
                        KeyState.Ctrl = false;
                        break;
                }
            }
            #endregion

            #region Keyの取得・変換
            char Key = (char)KeyBoardStruct.vkCode;
            Key = KeyState.Shift == true ? Char.ToUpper(Key) : Char.ToLower(Key);
            #endregion

            KeyHookData data = new KeyHookData(
                KeyBoardStruct.vkCode,
                Key,
                KeyState.Shift,
                KeyState.Ctrl,
                wParam.Equals(new IntPtr((int)HookKeyState.KeyUp)));

            #region グローバルフィルタ
            if (null != Filter)
                if (false == Filter(data))
                    return (IntPtr)1;
            #endregion

            if (KeyBoardStruct.vkCode != (int)HookKeyCodes.Shift &&
                KeyBoardStruct.vkCode != (int)HookKeyCodes.Ctrl)
            {
                #region 各種コールバック呼び出し
                if (KeyBoardEvent != null)
                    KeyBoardEvent(data);

                if (KeyDown != null)
                    if (data.KeyDown)
                        KeyDown(data);

                if (KeyUp != null)
                    if (data.KeyUp)
                        KeyUp(data);
                #endregion
            }

            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// キーボードのフックがすでにされているかどうか
        /// </summary>
        public static bool IsHooked { get { return hHook != IntPtr.Zero; } }
    }
}
