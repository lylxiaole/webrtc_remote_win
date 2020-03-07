
using Controls;
using Controls.Dialogs;
using Gma.System.MouseKeyHook;
using LYL.Common;
using LYL.Logic;
using LYL.Logic.Machine;
using MaterialDesignThemes.Wpf;
using MouseKeyPlayback;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UserMoudle.RemoteWindow.ToolBarManager;
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.RemoteWindow
{
    public partial class ControllerWindowView : CommonWindow
    {
        private KeyboardHook keyboardHook = new KeyboardHook();
        private MouseHook mouseHook = new MouseHook(); 
        private Record mouseMoveRecord = null;

        private ControllerWindowViewModel CurrentContext
        {
            get
            {
                return this.DataContext as ControllerWindowViewModel;
            }
        }

        public ControllerWindowView()
        {
            InitializeComponent();
            this.DataContext = new ControllerWindowViewModel();
            this.CurrentContext.RemoteImageControl = this.imgcontrol;
            this.CurrentContext.onClose += CurrentContext_onClose;
            //**********************************************
            this.Loaded += ControllerWindowView_Loaded;
        }

        private void ControllerWindowView_Loaded(object sender, RoutedEventArgs e)
        {
            keyboardHook.OnKeyboardEvent += KeyboardHook_OnKeyboardEvent;
            mouseHook.OnMouseEvent += MouseHook_OnMouseEvent;
            mouseHook.OnMouseMove += MouseHook_OnMouseMove;
            mouseHook.OnMouseWheelEvent += MouseHook_OnMouseWheelEvent;
            //
            StartWatch();
            StartSendMouseMoveEvents();
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                this.StopWatch();
                CurrentContext?.CloseAllConnetions();
            }
            catch (Exception)
            {
            }
            base.OnClosed(e);
        }
        private void CurrentContext_onClose(object sender, EventArgs e)
        {
            this.Close();
        }

        #region peerconnection处理
        /// <summary>
        /// 主动连接对方，创建offer发送给对方
        /// </summary>
        /// <param name="mchineId"></param>
        public void ConnectMachine(string mchineId)
        {
            this.CurrentContext.ConnectRemote(mchineId);
        }

        /// <summary>
        /// 主动连接对方成功，接受对方发来的answer
        /// </summary>
        /// <param name="mchineId"></param>
        /// <param name="offerinfo"></param>
        public void SetRemoteAnswer(string mchineId, SdpInfo remoteAnswer)
        {
            this.CurrentContext.SetRemoteAnswer(mchineId, remoteAnswer.sdp, remoteAnswer.type);
        }

        public void onRemoteClientSdpCompleted(string machineId)
        {
            this.CurrentContext.onRemoteClientSdpCompleted(machineId);
        }

        /// <summary>
        /// 接收到对方发来的IceCandite，进行处理
        /// </summary>
        /// <param name="mchineId"></param>
        /// <param name="iceCandidate"></param>
        public void AddRemoteIceCandite(string mchineId, IceCandidate iceCandidate)
        {
            this.CurrentContext.AddRemoteIceCandite(mchineId, iceCandidate);
        }
        #endregion

        #region 鼠标键盘事件的事件    
        private bool isSendingEvents { get; set; }
        private void StartSendMouseMoveEvents()
        {
            isSendingEvents = true;
            Task.Run(() =>
            {
                while (isSendingEvents)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    { 
                        if (this.mouseMoveRecord != null)
                        {
                            this.CurrentContext.SendWinEvent(mouseMoveRecord);
                            this.mouseMoveRecord = null;
                        }
                    });
                    Thread.Sleep(30);
                }
            });
        }
        /// <summary>
        /// 判断键盘是不是按下状态，true是按下状态
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private bool GetKeyState(Keys keys)
        {
            return ((MouseHook.GetKeyState((int)keys) & 0x8000) != 0) ? true : false;
        }
        private bool GetKeyState(int keys)
        {
            return ((MouseHook.GetKeyState((int)keys) & 0x8000) != 0) ? true : false;
        }

        public static int GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - DateTime.UtcNow.AddDays(-3);
            return Convert.ToInt32(ts.TotalMilliseconds);
        }

        private bool ProcessMouseEvent(MouseHook.MouseEvents mAction, int mValue)
        {
            var windowtitle = Win32Utils.GetActiveWindowTitle();
            if (windowtitle != this.Title)
            {
                return false;
            }

            CursorPoint mPoint = GetCurrentMousePosition();
            if (mPoint == null)
            {
                return false;
            }
            MouseEvent mEvent = new MouseEvent
            {
                Location = mPoint,
                Action = mAction,
                Value = mValue
            };
            //Console.WriteLine("添加鼠标事件:" + JsonConvert.SerializeObject(mEvent));
            LogMouseEvents(mEvent);
            return false;
        }

        private bool ProcessKeyboardEvent(BaseHook.KeyState keyState, uint key)
        {
            var windowtitle = Win32Utils.GetActiveWindowTitle();
            if (windowtitle != this.Title)
            {
                return false;
            }
            CursorPoint mPoint = GetCurrentMousePosition();
            if (mPoint == null)
            {
                return false;
            }

            if (this.isInterceptKey((Keys)key))
            {
                return false;
            }

            KeyboardEvent kEvent = new KeyboardEvent
            {
                Key = (Keys)key,
                Action = (keyState == BaseHook.KeyState.Keydown) ? Constants.KEY_DOWN : Constants.KEY_UP
            };

            //Console.WriteLine("添加键盘事件:" + JsonConvert.SerializeObject(kEvent));
            LogKeyboardEvents(kEvent);
            return false;
        }

        private void LogMouseEvents(MouseEvent mEvent)
        {
            Record item = new Record
            {
                Id = GetTimeStamp(),
                EventMouse = mEvent,
                Type = Constants.MOUSE,
                Content = ""
            };

            if (mEvent.Action == MouseHook.MouseEvents.MouseMove)
            {
                mouseMoveRecord = item;
            }
            else
            {
                this.CurrentContext.SendWinEvent(item);
            }
        }

        private void LogKeyboardEvents(KeyboardEvent kEvent)
        {
            Record item = new Record
            {
                Id = GetTimeStamp(),
                Type = Constants.KEYBOARD,
                EventKey = kEvent,
                Content = ""
            };
            this.CurrentContext.SendWinEvent(item);
        }

        //键盘拦截
        private bool isInterceptKey(Keys currentkey)
        {
            if (currentkey == Keys.Delete)
            {
                //拦截ctrl+alt+delete 
                if ((GetKeyState(Keys.LMenu) || GetKeyState(Keys.RMenu)) && (GetKeyState(Keys.LControlKey) || GetKeyState(Keys.RControlKey)))
                {
                    KeyboardEvent kEvent2 = new KeyboardEvent
                    {
                        Key = Keys.LControlKey,
                        Action = Constants.KEY_UP
                    };
                    LogKeyboardEvents(kEvent2);
                    //
                    KeyboardEvent kEvent3 = new KeyboardEvent
                    {
                        Key = Keys.RControlKey,
                        Action = Constants.KEY_UP
                    };
                    LogKeyboardEvents(kEvent3);
                    //
                    KeyboardEvent kEvent4 = new KeyboardEvent
                    {
                        Key = Keys.LMenu,
                        Action = Constants.KEY_UP
                    };
                    LogKeyboardEvents(kEvent4);
                    //
                    KeyboardEvent kEvent5 = new KeyboardEvent
                    {
                        Key = Keys.RMenu,
                        Action = Constants.KEY_UP
                    };
                    LogKeyboardEvents(kEvent5);
                    return true;
                }
            }
            return false;
        }

        private bool KeyboardHook_OnKeyboardEvent(uint key, BaseHook.KeyState keyState)
        {
            return ProcessKeyboardEvent(keyState, key);
        }

        private bool MouseHook_OnMouseWheelEvent(int wheelValue)
        {
            return ProcessMouseEvent((MouseHook.MouseEvents)wheelValue, 120);
        }

        private bool MouseHook_OnMouseMove(int x, int y)
        {
            return ProcessMouseEvent(MouseHook.MouseEvents.MouseMove, 0);
        }

        private bool MouseHook_OnMouseEvent(int mouseEvent)
        {
            return ProcessMouseEvent((MouseHook.MouseEvents)mouseEvent, 0);
        }

        private CursorPoint GetCurrentMousePosition()
        {
            var mousePoint = new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);
            var imgrect = this.GetAbsolutePlacement(this.imgcontrol);
            if (!imgrect.Contains(mousePoint))
            {
                return null;
            }

            var xPersent = (mousePoint.X - imgrect.Left) / imgrect.Width;
            var yPersent = (mousePoint.Y - imgrect.Top) / imgrect.Height;
            return new CursorPoint(xPersent, yPersent);
        }

        public Rect GetAbsolutePlacement(FrameworkElement element)
        {
            if (System.Windows.PresentationSource.FromVisual(element) == null)
            {
                return new Rect(0, 0, 0, 0);
            }

            Matrix transformToDevice = System.Windows.PresentationSource.FromVisual(element).CompositionTarget.TransformToDevice;
            var DPIX = transformToDevice.M11;
            var DPIY = transformToDevice.M22;

            var absolutePos = element.PointToScreen(new System.Windows.Point(0, 0));
            return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth / (1 / DPIX), element.ActualHeight / (1 / DPIX));
        }

        public void StopWatch()
        {
            isSendingEvents = false;
            keyboardHook.Uninstall();
            mouseHook.Uninstall();
        }
        public void StartWatch()
        {
            keyboardHook.Install();
            mouseHook.Install();
        }
        #endregion

        #region 快捷操作栏 
        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.controlList.Visibility = Visibility.Visible;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.controlList.Visibility = Visibility.Collapsed;
                this.quickBar.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        private void TopControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void PackIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.quickBar.Visibility == Visibility.Collapsed)
            {
                this.quickBar.Visibility = Visibility.Visible;
                if (this.WindowState == WindowState.Maximized)
                {
                    this.controlList.Visibility = Visibility.Visible;
                } 
            }
            else
            {
                this.quickBar.Visibility = Visibility.Collapsed;
                if (this.WindowState == WindowState.Maximized)
                {
                    this.controlList.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
