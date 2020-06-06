using Dispath;
using MaterialDesignThemes.Wpf;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Controls.Dialogs
{

    public class DialogBase : UserControl
    {
    }
     
    public static class DialogExtension   //  必须是一个静态类
    { 
        public static Task<object> PopupDialog(this UserControl con, object dialogIdentifier = null, DialogOpenedEventHandler openedEventHandler = null, DialogClosingEventHandler closingEventHandler = null)    //必须为public static 类型，且参数使用this关键字
        { 
            return Application.Current.Dispatcher.Invoke<Task<object>>(() =>
            {
                return DialogHost.Show(con, dialogIdentifier, openedEventHandler, closingEventHandler);
            });
        }

        public static void CloseDialog(this UserControl con, object dialogIdentifier = null, DialogOpenedEventHandler openedEventHandler = null, DialogClosingEventHandler closingEventHandler = null)    //必须为public static 类型，且参数使用this关键字
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DialogHost.CloseDialogCommand.Execute(con, con);
                //return DialogHost.close(con, dialogIdentifier, openedEventHandler, closingEventHandler);
            });
        }

        public static IPendingHandler ShowLoadingWindow(this ViewModelBase vm)
        {
            var handler = PendingBox.Show("请等待...", "等待窗", false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
                LoadingSize = 50,
                PendingBoxStyle = PendingBoxStyle.Classic,
                FontSize = 14,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                InteractOwnerMask=true
                
            });
            return handler;
        }

        public static void ShowMessageBoxError(this DispatcherObject con,string message,string title, MessageBoxButton boxbtn)
        {
            var result = MessageBoxX.Show(message, title, Application.Current.MainWindow, boxbtn, new MessageBoxXConfigurations()
            {
                MessageBoxStyle = MessageBoxStyle.Classic,
                MessageBoxIcon = MessageBoxIcon.Error,
                ButtonBrush = "#FF4C4C".ToColor().ToBrush(),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                InteractOwnerMask = true
            });
        }
        public static void ShowMessageBoxError(this ViewModelBase con, string message, string title, MessageBoxButton boxbtn)
        {
            var result = MessageBoxX.Show(message, title, Application.Current.MainWindow, boxbtn, new MessageBoxXConfigurations()
            {
                MessageBoxStyle = MessageBoxStyle.Classic,
                MessageBoxIcon = MessageBoxIcon.Error,
                ButtonBrush = "#FF4C4C".ToColor().ToBrush(),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                InteractOwnerMask = true
            });
        }

        public static void ShowMessageBoxSuccess(this ViewModelBase con, string message, string title, MessageBoxButton boxbtn)
        {
            var result = MessageBoxX.Show(message, title, Application.Current.MainWindow, boxbtn, new MessageBoxXConfigurations()
            {
                MessageBoxStyle = MessageBoxStyle.Classic,
                MessageBoxIcon = MessageBoxIcon.Success,
                ButtonBrush = "#FF4C4C".ToColor().ToBrush(),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                InteractOwnerMask = true
            });
        }
    }
}
