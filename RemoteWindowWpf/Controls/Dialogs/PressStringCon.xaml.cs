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
    /// PressStringCon.xaml 的交互逻辑
    /// </summary>
    public partial class PressStringCon : DialogBase
    {
        public PressStringCon()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        public string InputString { get; set; }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.InputString = "";
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void Button_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
        {
            this.InputString = this.inputtext.Text;
            DialogHost.CloseDialogCommand.Execute(this, null);
        }
    }
}
