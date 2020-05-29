using Controls;
using Dispath.MoudleInterface;
using LYL.Logic.Machine;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using ShowMeTheXAML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace LYLRemote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : CommonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded; 
        }



        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationHelper.RegisterContentControl(this.content);
            NavigationHelper.NavigatedToView("用户登录");
            try
            {
                var machineInfo = MachineLogic.localMachine();
                var netmachineInfo = UserLogic.AutoLogin(machineInfo.token);
                if (netmachineInfo == null)
                {
                    NavigationHelper.NavigatedToView("用户登录");
                }
                if (netmachineInfo.machineId != machineInfo.machineId)
                {
                    NavigationHelper.NavigatedToView("用户登录");
                }
                machineInfo.machineName = netmachineInfo.machineName;
                machineInfo.machinepwd = netmachineInfo.machinePwd;
                MachineLogic.RecordMachineInfo(machineInfo);
                NavigationHelper.NavigatedToView("主界面");
            }
            catch (Exception)
            {
                NavigationHelper.NavigatedToView("用户登录");
            }

            var Swatches = new SwatchesProvider().Swatches;
            var swatch = Swatches.FirstOrDefault(o => o.Name == "blue");
            ModifyTheme(theme =>
            {
                theme.SetBaseTheme(Theme.Light);
            });
            ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));
            ModifyTheme(theme => theme.SetSecondaryColor(swatch.AccentExemplarHue.Color));
            ///////////////////////////////
            /// 
        }


        private static void ModifyTheme(Action<ITheme> modificationAction)
        {

            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }


        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                if(this.notifyIcon.Visible==false)
                {
                    this.MakeSystemIcon();
                } 
            }
            catch (Exception ex)
            {
            } 

            e.Cancel = true;
            this.ShowInTaskbar = false;
            this.Hide();
            base.OnClosing(e);
        }


        #region 创建系统右键菜单
        NotifyIcon notifyIcon = new NotifyIcon();
        private void MakeSystemIcon()
        {
            this.notifyIcon.BalloonTipText = "LYLRemote正在运行..."; //设置程序启动时显示的文本
            this.notifyIcon.Text = "LYLRemote";//最小化到托盘时，鼠标点击时显示的文本

            //this.notifyIcon.Icon = new System.Drawing.Icon("pack://application:,,,/LYL.Common;component/images/timg.ico");
            this.notifyIcon.Icon = new System.Drawing.Icon(System.AppDomain.CurrentDomain.BaseDirectory + "timg.ico");//程序图标
            this.notifyIcon.Visible = true;
            //右键菜单--打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("主界面");
            open.Click += new EventHandler(ShowWindow);
            //右键菜单--退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(CloseWindow);
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);
            //
            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.ShowBalloonTip(1000);
        }

        private void OnNotifyIconDoubleClick(object sender, MouseEventArgs e)
        {
            _showMainWindow();
        }

        private void ShowWindow(object sender, EventArgs e)
        {
            //this.Visibility = System.Windows.Visibility.Visible;
            _showMainWindow();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void _showMainWindow()
        {
            this.ShowInTaskbar = true;
            this.Topmost = false;
            this.Topmost = true;
            this.WindowState = WindowState.Normal;
            this.Show();
        }
        #endregion



    }
}
