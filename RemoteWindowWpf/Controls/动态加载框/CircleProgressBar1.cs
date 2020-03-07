using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{


    //用例 
    //    <con:CircleProgressBar1  Height="100"
    //                 Margin="140,124,0,0"
    //                 VerticalAlignment="Top"
    //                  Maximum="360" HorizontalAlignment="Left" Width="100" ></con:CircleProgressBar1>

    public class CircleProgressBar1 : ProgressBar
    {
        public CircleProgressBar1()
        {
            this.Style = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/动态加载框/CircleProgressBarStyle.xaml", "ProgressBarStyleCircle");
            this.Loaded += CircleProgressBar1_Loaded;
        }

        private void CircleProgressBar1_Loaded(object sender, RoutedEventArgs e)
        {
            Task th = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Value += 1;

                    }));
                    Thread.Sleep(500);
                }
            });
            th.Start();
        }
    }
}
