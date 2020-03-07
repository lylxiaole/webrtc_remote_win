using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class PropertyGrid : DataGrid
    {
        public PropertyGrid() : base()
        {
            this.Loaded += PropertyGrid_Loaded;

            this.Style = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/属性窗/PropertyGridResource.xaml", "propertygrid");
        }

        private void PropertyGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        { 
            
        }

        /// <summary>
        /// 为属性窗加载数据
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(object data)
        {

        }

    }
}
