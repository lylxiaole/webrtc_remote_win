using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class LYLTabItem : TabItem
    {
        public LYLTabItem()
        {
            this.Style = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/TabControl标签控件/LYLTabControl1.xaml", "aytabitem");
        }
    }
}
