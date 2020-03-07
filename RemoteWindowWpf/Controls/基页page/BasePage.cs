using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
   public class BasePage:Page
    {
        public BasePage()
        {
            this.Style= SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/基页page/BasePageSource.xaml", "basepagestyle1");
        }
    }
}
