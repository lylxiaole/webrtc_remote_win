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

namespace Controls.Dialogs
{
    /// <summary>
    /// ChangeMachineNameCon.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeMachineNameCon : DialogBase
    {
        public ChangeMachineNameCon()
        {
            InitializeComponent();
        }
        public ChangeMachineNameCon(string oldName)
        {
            InitializeComponent();
            this.oldName.Text = oldName;
        }

        public string NewText
        {
            get
            {
                return this.newName.Text;
            }
        }


        public string OldText
        {
            get
            {
                return this.oldName.Text;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string oldname = this.oldName.Text;
            string newname = this.newName.Text; 
            DialogHost.CloseDialogCommand.Execute(this, this.newName);
        }
    }
}
