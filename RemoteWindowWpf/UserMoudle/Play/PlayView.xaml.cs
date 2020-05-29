using Dispath.MoudleInterface;
using LYL.Logic.Machine;
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
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Threading;

namespace UserMoudle.Play
{
    /// <summary>
    /// PlayView.xaml 的交互逻辑
    /// </summary>
    /// 
    [View(MoudleName = "直播主页面")]
    public partial class PlayView : System.Windows.Controls.UserControl, INormalView
    {
        public PlayViewModel CurrentContext { get { return this.DataContext as PlayViewModel; } }

        public PlayView()
        {
            InitializeComponent();
            this.DataContext = new PlayViewModel();
            this.Loaded += PlayView_Loaded;
            this.Unloaded += PlayView_Unloaded;
        }

        private void PlayView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.CurrentContext.ClosePlay();
        }

        private void PlayView_Loaded(object sender, RoutedEventArgs e)
        {
            this.CurrentContext.LocalImageControl = this.imgcontrol;
            this.playtext.Text = this.CurrentContext.StartPlay();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentContext.ClosePlay();

        }
    }
}
