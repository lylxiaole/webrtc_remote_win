
using Controls.Dialogs;
using EncodeousCommon.Sys.Windows;
using LYL.Common;
using LYL.Common.WindowServiceManager;
using LYL.Logic.Machine;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using ShowMeTheXAML;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace LYLRemote
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : System.Windows.Application
    {
        MainWindow currentMainWindow
        {
            get
            {
                return this.MainWindow as MainWindow;
            }
        }
        WebSocketClient wsclient = new WebSocketClient();
        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            XamlDisplay.Init();
            this.Startup += App_Startup;
            this.Activated += App_Activated;
        }

        private void App_Activated(object sender, EventArgs e)
        {
            //

        }

        private void App_Startup(object sender, StartupEventArgs e)
        {

        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ErrorDialogCon con = new ErrorDialogCon();
            con.ErrorMsg = e.Exception.Message;
            //show the dialog
            con.PopupDialog();
        }
    }
}
