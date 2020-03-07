using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class LYLTabControl:TabControl
    {
        public LYLTabControl()
        {
            this.Style = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/TabControl标签控件/LYLTabControl1.xaml", "AyTabControl");
       

            //this.ItemContainerStyle = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/TabControl标签控件/LYLTabControl1.xaml", "listTabItemStyle"); 
        }
    }
}
