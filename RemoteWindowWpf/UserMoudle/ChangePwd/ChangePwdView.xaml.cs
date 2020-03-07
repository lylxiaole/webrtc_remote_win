using Dispath.MoudleInterface;
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

namespace UserMoudle.ChangePwd
{
    /// <summary>
    /// ChangePwdView.xaml 的交互逻辑
    /// </summary>
    /// 
    [View(MoudleName = "修改密码")]
    public partial class ChangePwdView : UserControl, INormalView
    {
        public ChangePwdView()
        {
            InitializeComponent();
            this.DataContext = new ChangePwdViewModel();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationHelper.NavigatedToView("用户登录");
        }
    }
}
