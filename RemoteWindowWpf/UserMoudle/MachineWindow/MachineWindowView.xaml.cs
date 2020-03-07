using Controls.Dialogs;
using Dispath.MoudleInterface;
using LYL.Logic.Machine;
using MaterialDesignThemes.Wpf; 
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

namespace UserMoudle.MachineWindow
{
    /// <summary>
    /// MachineWindowView.xaml 的交互逻辑
    /// </summary>
    /// 
    [View(MoudleName = "主界面")]
    public partial class MachineWindowView : UserControl, INormalView
    {
        public MachineWindowView()
        {
            InitializeComponent(); 
            this.DataContext = new MachineWindowViewModel();
        }

        public MachineWindowViewModel CurrentContext
        {
            get
            {
                return this.DataContext as MachineWindowViewModel;
            }
        }
         
        #region 快捷操作按钮 
        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        { 
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        { 
        }

        private void ChangeMachineNameButton_OnClick(object sender, RoutedEventArgs e)
        {
            ChangeMachineNameCon popup = new ChangeMachineNameCon(MachineLogic.localMachine().machineName);
            popup.PopupDialog(closingEventHandler: this.onChangeMachineNamePopupClosed);
        }

        private void onChangeMachineNamePopupClosed(object sender, DialogClosingEventArgs eventArgs)
        {
            ChangeMachineNameCon popup = eventArgs.Parameter as ChangeMachineNameCon;
            var newName = popup.NewText;
            var oldName = popup.OldText;
            this.CurrentContext.ChangeMachineName(newName, oldName); 
        }

        #endregion
    }
}
