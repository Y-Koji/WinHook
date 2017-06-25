using System;
using System.Runtime.InteropServices;

namespace HookLib.Data
{
    namespace Mouse
    {
        /// <summary>
        /// マウスの座標構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// マウス動作種類の列挙体
        /// </summary>
        public enum Click : int
        {
            None = 512,
            LeftDown = 513, LeftUp = 514,
            RightDown = 516, RightUp = 517,
            WheelDown = 519, WheelUp = 520,
        }

        public enum Scroll : uint
        {
            Up = 7864320, Down = 4287102976,
        }

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

    namespace Keyboard
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

        public enum HookKeyCodes
        {
            Shift = 160, Ctrl = 162,
        }

        public enum HookKeyState
        {
            KeyDown = 256, KeyUp = 257,
        }

        public class KeyHookData
        {
            public KeyHookData(int code, char key, bool shift, bool ctrl, bool keyup)
            {
                this.Code = code;
                this.Key = key;
                this.Shift = shift;
                this.Ctrl = ctrl;
                this.KeyUp = keyup;
            }

            /// <summary>
            /// イベントで発生したキーに対応するキーコード
            /// </summary>
            public int Code { get; private set; }
            /// <summary>
            /// イベントで発生したキーに対応する文字
            /// </summary>
            public char Key { get; private set; }
            /// <summary>
            /// Shiftキーが押下されているか否か
            /// </summary>
            public bool Shift { get; private set; }
            /// <summary>
            /// Ctrlキーが押下されているか否か
            /// </summary>
            public bool Ctrl { get; private set; }
            /// <summary>
            /// キーが離されたのかどうか
            /// </summary>
            public bool KeyUp { get; private set; }
            /// <summary>
            /// キーが押下されたのかどうか
            /// </summary>
            public bool KeyDown { get { return !KeyUp; } }
        }
    }
}
