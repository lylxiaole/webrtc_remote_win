using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dispath.MoudleInterface
{
    public class NavigationHelper
    {
        public static void RegisterContentControl(ContentControl content)
        {
            _content = content;
        }
        static ContentControl _content = null;

        public static void NavigatedToPage(string name)
        {
            var page = MoudleCommon.PageViews.FirstOrDefault(o => o.MoudleName == name);
            if (page != null)
            {
                NavigatedToMoudle(page);
            }
        }

        public static void NavigatedToView(string name)
        {
            var page = MoudleCommon.NormalViews.FirstOrDefault(o => o.MoudleName == name);
            if (page != null)
            {
                NavigatedToMoudle(page);
            }
        }

        public static void NavigatedToWindow(string name)
        {
            var page = MoudleCommon.WindowViews.FirstOrDefault(o => o.MoudleName == name);
            if (page != null)
            {
                NavigatedToMoudle(page);
            }
        }


        private static void NavigatedToMoudle(MoudleInfo moudle)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var control = Activator.CreateInstance(moudle.MoudleType);
                if (control is INormalView)
                {
                    var con = control as FrameworkElement;
                    _content.Content = con;
                }

                if (control is IPage)
                {
                    var con = control as FrameworkElement;
                    _content.Content = con;
                }

                if (control is IWindowView)
                {
                    var win = control as Window;
                    win.ShowDialog();
                }
            }));
        }
    }
}
