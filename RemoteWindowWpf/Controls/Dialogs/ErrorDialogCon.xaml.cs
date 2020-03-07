using Controls.Dialogs;
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
    /// ErrorDialogCon.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorDialogCon : DialogBase
    {
        public ErrorDialogCon()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string ErrorMsg { get; set; }

    }
}
