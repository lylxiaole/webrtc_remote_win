using Controls;
using Dispath.MoudleInterface;
using LYL.Logic.Machine;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using ShowMeTheXAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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


namespace RemoteWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : CommonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            XamlDisplay.Init();
            NavigationHelper.RegisterContentControl(this.content);
            NavigationHelper.NavigatedToView("用户登录");
             
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
    }
}
