using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
   public class DataGrid1:DataGrid
    {
        public DataGrid1()
        {
            this.Style = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/各种表格/DataGrid.xaml", "DataGridStyle");
        
        }
    }
}
