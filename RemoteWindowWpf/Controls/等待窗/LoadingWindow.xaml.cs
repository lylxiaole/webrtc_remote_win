using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Controls
{
    /// <summary>
    /// LoadingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.Top = int.MaxValue;
            this.Topmost = true;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.WindowState = WindowState.Normal;
            var bar = new FluidProgressBar();
            bar.Foreground = Brushes.White;
            this.Content = bar;
        }
        static Thread th;
        static object obj = new object();

        public static void ShowDialogger()
        {
            th = new Thread(() =>
          {
              if (th.ThreadState !=  ThreadState.Running) { return; }
              LoadingWindow win = new LoadingWindow();  //{Owner=Application.Current.MainWindow}
              win.ShowDialog();
          });
            th.SetApartmentState(ApartmentState.STA);
            th.IsBackground = true;
            th.Start();
        }

        public static void CloseDialogger()
        {
            
            th.Abort();
            th.DisableComObjectEagerCleanup();
        }

    }
}
