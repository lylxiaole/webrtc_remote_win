using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class MyComobox : ComboBox
    {
        public MyComobox()
        {
            this.Style = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/下拉框/ComoboxDictionary.xaml", "DefaultComboBox");
        }
    }
}
