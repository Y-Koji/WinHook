using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using HookLib;
using HookLib.Data;
using HookLib.Data.Mouse;

namespace MouseHoldEvent
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Not working.";
            Console.WriteLine("C + WheelDown");
            Console.WindowWidth = 30;
            Console.WindowHeight = 1;

            bool click_toggle = false;

            MouseHook.WheelDown += () =>
            {
                if (Keyboard.IsKeyDown(Key.C))
                {
                    if (click_toggle = !click_toggle)
                    {
                        Console.Title = "Running";
                    }
                    else
                    {
                        Console.Title = "Not working.";
                    }
                }
            };

            MouseHook.Set();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (click_toggle)
                    {
                        MouseHook.Rize(MouseEvent.LEFTDOWN);
                        MouseHook.Rize(MouseEvent.LEFTUP);
                        Task.Delay(200).Wait();
                    }
                }
            }).Wait();
        }
    }
}
