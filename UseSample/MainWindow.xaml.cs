using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HookLib;
using HookLib.Data.Keyboard;
using HookLib.Data.Mouse;

namespace UseSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            #region キーボードイベントが来た時に右側Listにデータを出力する処理
            KeyboardHook.KeyBoardEvent += (e) =>
                Dispatcher.Invoke(() => KeyboardHookInfo.Items.Insert(0, e.Key));
            #endregion

            #region マウスイベントが来た時に左側Listにデータを出力する処理
            MouseHook.MouseEvent += (pt, type) =>
            {
                if (type is Click)
                {
                    switch ((Click)type)
                    {
                        case Click.None:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, pt.x + ", " + pt.y));
                            break;
                        case Click.LeftDown:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, "LEFT DOWN: " + pt.x + ", " + pt.y));
                            break;
                        case Click.LeftUp:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, "LEFT UP:" + pt.x + ", " + pt.y));
                            break;
                        case Click.RightDown:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, "RIGHT DOWN: " + pt.x + ", " + pt.y));
                            break;
                        case Click.RightUp:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, "RIGHT UP: " + pt.x + ", " + pt.y));
                            break;
                        case Click.WheelDown:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, "WHEEL DOWN: " + pt.x + ", " + pt.y));
                            break;
                        case Click.WheelUp:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, "WHEEL UP: " + pt.x + ", " + pt.y));
                            break;
                    }
                }
                else if (type is Scroll)
                {
                    switch ((Scroll)type)
                    {
                        case Scroll.Up:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, "SCROLL UP: " + pt.x + ", " + pt.y));
                            break;
                        case Scroll.Down:
                            Dispatcher.Invoke(() => MouseHookInfo.Items.Insert(0, "SCROLL DOWN: " + pt.x + ", " + pt.y));
                            break;
                    }
                }
            };
            #endregion

            #region 右クリックを無効にするフィルタ
            MouseHook.Filter += (pt, type) =>
            {
                if (type is Click)
                {
                    switch ((Click)type)
                    {
                        case Click.RightDown:
                            return false;
                        case Click.RightUp:
                            return false;
                        default:
                            return true;
                    }
                }
                else
                {
                    return true;
                }
            };
            #endregion

            #region 'a'入力を無効にするフィルタ
            KeyboardHook.Filter += (e) =>
            {
                if ('a' == e.Key)
                    return false;
                else
                    return true;
            };
            #endregion

            #region マウスフックの有効/無効ボタン動作
            bool mHooked = false;
            MouseHookButton.Click += (sender, e) =>
            {
                if (mHooked)
                {
                    mHooked = false;
                    MouseHook.Remove();
                    MouseHookButton.Content = "Mouse Hook";
                }
                else
                {
                    mHooked = true;
                    MouseHook.Set();
                    MouseHookButton.Content = "Mouse UnHook";
                }
            };
            #endregion

            #region キーボードフックの有効/無効ボタン動作
            bool kHooked = false;
            KeyboardHookButton.Click += (sender, e) =>
            {
                if (kHooked)
                {
                    kHooked = false;
                    KeyboardHook.Remove();
                    KeyboardHookButton.Content = "Keyboard Hook";
                }
                else
                {
                    kHooked = true;
                    KeyboardHook.Set();
                    KeyboardHookButton.Content = "Keyboard UnHook";
                }
            };
            #endregion

        }
    }
}
