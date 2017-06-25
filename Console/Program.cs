using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HookLib;
using HookLib.Data;
using HookLib.Data.Mouse;

namespace Console
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MouseHook.MouseEvent += MouseHook_MouseEvent;
            MouseHook.WheelDown += MouseHook_WheelDown;


            MouseHook.Set();

            while (true)
                Task.Delay(60000).Wait();
        }

        static void MouseHook_WheelDown()
        {
            throw new NotImplementedException();
        }

        static void MouseHook_MouseEvent(POINT arg1, Enum arg2)
        {
            System.Console.WriteLine(arg1);
        }
    }
}
