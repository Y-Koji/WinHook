using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinHook.Keyboard
{
    class KeyboardHook : Hook
    {
        protected override IntPtr Procedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KeyBoardHookStruct KeyBoardStruct = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));

            _chain?.Invoke((KeyCode)KeyBoardStruct.vkCode);
            _chain?.Invoke((char)KeyBoardStruct.vkCode);
            _chain?.Invoke((KeyState)wParam);

            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }
    }
}
