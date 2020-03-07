using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Controls
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageControl : Viewbox
    {
        public ImageControl()
        {
            InitializeComponent();
            this.Loaded += ImageControl_Loaded; 
        }
        TransformGroup group;
        TranslateTransform TranslateTransform;
        ScaleTransform ScaleTransform;
        private void ImageControl_Loaded(object sender, RoutedEventArgs e)
        {
            group= this.FindResource("ImageTransformResource") as TransformGroup;
            ScaleTransform = group.Children[0] as ScaleTransform;
            TranslateTransform = group.Children[1] as TranslateTransform;
        }


        private Point m_PreviousMousePoint;
        private void MasterImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
 

            m_PreviousMousePoint = e.GetPosition(rectangle);
        }

        private void MasterImage_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            if (rectangle == null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(rectangle);
                TranslateTransform.X += position.X - m_PreviousMousePoint.X;
                TranslateTransform.Y += position.Y - m_PreviousMousePoint.Y;
                m_PreviousMousePoint = position;
            }
        }


        private void MasterImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScaleTransform.ScaleX += e.Delta * 0.001;
            ScaleTransform.ScaleY += e.Delta * 0.001;
        }

        private void MasterImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
          
    
        }
    }
}
