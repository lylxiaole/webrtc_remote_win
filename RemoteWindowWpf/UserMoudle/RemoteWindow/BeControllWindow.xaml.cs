using Controls;
using Controls.Dialogs;
using LYL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using UserMoudle.RemoteWindow.FileSendManager;

namespace UserMoudle.RemoteWindow
{
    /// <summary>
    /// BeControllWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BeControllWindow : CommonWindow
    {
        public RemoteBeingControlled becontrolContext { get; set; }
        public ToolBarManagerViewModel toolbarManager { get; set; } = new ToolBarManagerViewModel();

        public bool isTicks { get; set; } = true;
        public Action recordAction { get; set; }
        public BeControllWindow()
        {
            InitializeComponent(); 
        }

        public BeControllWindow(RemoteBeingControlled becontrol)
        {
            InitializeComponent();
            this.becontrolContext = becontrol;
            this.toolbarManager.setControlContext(becontrol);
            this.DataContext = this;

            //recordAction = () =>
            //{
            //    this.ticks.Text = DateTime.Now.ToString("HH:mm:ss fff");
            //};
            //Task.Run(() =>
            //{
            //    while (isTicks)
            //    {
            //        this.Dispatcher.BeginInvoke(recordAction);
            //        Thread.Sleep(3);
            //    }
            //});

        }
        protected override void OnClosed(EventArgs e)
        {
            isTicks = false;
            base.OnClosed(e);
        }


    }
}
