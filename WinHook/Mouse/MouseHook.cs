using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinHook.Mouse
{
    class MouseHook : Hook
    {
        protected override IntPtr Procedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // ポインタ先の情報を構造体にマッピングする
            MouseHookStruct mouseStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            #region 各種コールバック呼び出し
            if (mouseStruct.mouseData == (uint)Scroll.Up ||
                mouseStruct.mouseData == (uint)Scroll.Down)
            {
                // マウスホイール操作を検出
                // スクロール情報で登録されているアクションを実行
                _chain?.Invoke((Scroll)mouseStruct.mouseData);
            }
            else if ((Click)wParam != Click.None)
            {
                // 何かクリックを検出
                // クリック情報で登録されているアクションを実行
                _chain?.Invoke((Click)wParam);
            }
            else
            {
                // その他であればマウスの移動を検出
                MouseMove move = new MouseMove();
                move.X = mouseStruct.pt.x;
                move.Y = mouseStruct.pt.y;

                // マウス移動情報で登録されているアクションを実行
                _chain?.Invoke(move);
            }
            #endregion

            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }
    }
}
