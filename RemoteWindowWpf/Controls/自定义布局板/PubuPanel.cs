using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
    public class PubuPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {

            foreach (var ele in base.InternalChildren)
            {
                Size size = new Size { Height = 40, Width = 40 };
            }
            return base.MeasureOverride(availableSize);
        }

        public static DependencyProperty BackgroundColorProperty;

        static PubuPanel()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(Colors.Yellow);
            metadata.AffectsRender = true;
            BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(PubuPanel), metadata);
        }

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty,value); }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            Rect bounds = new Rect(0,0,base.ActualWidth,base.ActualHeight);
            dc.DrawRectangle(GetForegroundBrush(),null,bounds);
           
        }

        public Brush GetForegroundBrush()
        {
            if(!IsMouseOver)
            {
                return new SolidColorBrush(BackgroundColor);
            }
            else
            {
                RadialGradientBrush brush = new RadialGradientBrush(Colors.White,BackgroundColor);
                Point absoluteGradientOrigin = Mouse.GetPosition(this);

                Point relativeGradientOrigin = new Point(
                    absoluteGradientOrigin.X/base.ActualWidth,
                    absoluteGradientOrigin.Y/base.ActualHeight);

                brush.GradientOrigin = relativeGradientOrigin;
                brush.Center = relativeGradientOrigin;

                return brush;
            }
        }
    }
}
