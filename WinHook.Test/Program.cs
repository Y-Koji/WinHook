using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHook.Keyboard;
using WinHook.Mouse;

namespace WinHook.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IHook mHook = Hook.Mouse;
            mHook.Set()
                .Is<MouseMove>(move => Console.WriteLine($"マウスの移動: {move}"))
                .Is<Click>(click => Console.WriteLine($"クリック操作: {click}"))
                .Is<Scroll>(scroll => Console.WriteLine($"ホイール操作: {scroll}"))
                .Any(obj => Console.WriteLine($"何か操作: {obj}"));

            IHook kHook = Hook.Keyboard;
            kHook.Set()
                .Is<KeyState>(state => Console.WriteLine($"KeyState: {state}"))
                .Is<KeyCode>(code => Console.WriteLine($"KeyCode: {code}"))
                .Is<char>(@char => Console.WriteLine($"Char: {@char}"))
                .Any(obj => Console.WriteLine($"何か操作: {obj}"));

            // 10秒待機
            Task.Delay(1000 * 10).Wait();
        }
    }
}
