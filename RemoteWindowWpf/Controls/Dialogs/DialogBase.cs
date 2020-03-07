using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

    }
}
