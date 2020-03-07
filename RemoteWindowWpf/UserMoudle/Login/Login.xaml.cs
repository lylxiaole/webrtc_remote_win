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

namespace UserMoudle
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    /// 
    [View(MoudleName = "用户登录")]
    public partial class Login : UserControl, INormalView
    {
        public Login()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(); 
        }

        private void Ripple_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            NavigationHelper.NavigatedToView("用户注册");
        }

        private void Ripple_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
        {
            NavigationHelper.NavigatedToView("修改密码");
        }
    }
}
