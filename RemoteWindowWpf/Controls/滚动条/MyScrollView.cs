using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class MyScrollView : ScrollViewer
    {
        public MyScrollView()
        {
            this.Template = SourceHelper.FindSourceByKey<ControlTemplate>(@"pack://application:,,,/Controls;component/滚动条/ScrollViewDictionary.xaml", "MyScrollViewerControlTemplate");
        }
    }
}
