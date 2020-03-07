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
    //   <con:CircleProgressBar2 Height = "100"
    //                 Margin="0,23.2,215.2,0"
    //                 VerticalAlignment="Top"
    //                  Maximum="360" HorizontalAlignment="Right" Width="100" ></con:CircleProgressBar2>

   public class CircleProgressBar2: ProgressBar
    {
        public CircleProgressBar2()
        {
            this.Style = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/动态加载框/CircleProgressBarStyle.xaml", "ProgressBarStyleFullCircle");
            this.Loaded += CircleProgressBar2_Loaded;
        }

        private void CircleProgressBar2_Loaded(object sender, RoutedEventArgs e)
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
