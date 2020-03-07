using Dispath;
using Dispath.MoudleInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Controls
{
    public class CommonWindow : Window, IWindowView
    {
        public CommonWindow()
        { 
            this.Style = SourceHelper.FindSourceByKey<Style>(@"pack://application:,,,/Controls;component/通用窗口/CommonWindowEffect.xaml", "BaseWindowStyle");
            this.DataContext = this;

            //窗体阴影效果
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.ResizeMode = ResizeMode.NoResize;
            this.BorderThickness = new Thickness(5);

            DropShadowEffect deff = new DropShadowEffect();
            deff.ShadowDepth = 1;
            deff.BlurRadius = 10;
            deff.Direction = 270;
            deff.Color = Brushes.Black.Color;
            deff.Opacity = 1;
            this.Effect = deff;
            this.MinWidth = 400;
            this.MinHeight = 400;

        }

        //背景图片
        public ImageSource BackgroundImage
        {
            get { return (ImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }
        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(CommonWindow), new PropertyMetadata(null));

         
        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        } 
        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(CommonWindow), new PropertyMetadata(Visibility.Visible));




        public Visibility BottomVisibility
        {
            get { return (Visibility)GetValue(BottomVisibilityProperty); }
            set { SetValue(BottomVisibilityProperty, value); }
        } 
        public static readonly DependencyProperty BottomVisibilityProperty =
            DependencyProperty.Register("BottomVisibility", typeof(Visibility), typeof(CommonWindow), new PropertyMetadata(Visibility.Visible));




        //窗体移动
        private ICommand _mouseMoveCommand;
        public ICommand MouseMoveCommand
        {
            get
            {
                if (_mouseMoveCommand == null)
                {
                    _mouseMoveCommand = new DelegateCommand((Param) =>
                    {
                        if (Mouse.LeftButton == MouseButtonState.Pressed)
                        {
                            DragMove();
                        }
                    });
                }
                return _mouseMoveCommand;
            }
        }
        //窗体标栏双击
        private ICommand _mouseDoubleClickCommand;
        public ICommand MouseDoubleClickCommand
        {
            get
            {
                if (_mouseDoubleClickCommand == null)
                {
                    _mouseDoubleClickCommand = new DelegateCommand((Param) =>
                    {
                        if (this.WindowState == WindowState.Normal)
                        {
                            this.WindowState = WindowState.Maximized;
                        }
                        else if (this.WindowState == WindowState.Maximized)
                        {
                            this.WindowState = WindowState.Normal;
                        }
                    });
                }
                return _mouseDoubleClickCommand;
            }
        }
        //关闭窗口
        private ICommand _closeWindowCommand;
        public ICommand CloseWindowCommand
        {
            get
            {
                if (_closeWindowCommand == null)
                {
                    _closeWindowCommand = new DelegateCommand((Param) =>
                    {
                        this.Close();
                    });
                }
                return _closeWindowCommand;
            }
        }
        //窗口最小化
        private ICommand _minWindowCommand;
        public ICommand MinWindowCommand
        {
            get
            {
                if (_minWindowCommand == null)
                {
                    _minWindowCommand = new DelegateCommand((Param) =>
                    {
                        this.WindowState = WindowState.Minimized;
                    });
                }
                return _minWindowCommand;
            }
        }



        #region 拖拽窗体改变大小
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitializeReSize();
        }

        void InitializeReSize()
        {
            var topResizer = GetTemplateChild<Thumb>("top");
            topResizer.DragDelta += new DragDeltaEventHandler(ResizeTop);

            var leftResizer = GetTemplateChild<Thumb>("left");
            leftResizer.DragDelta += new DragDeltaEventHandler(ResizeLeft);
            var rightResizer = GetTemplateChild<Thumb>("right");
            rightResizer.DragDelta += new DragDeltaEventHandler(ResizeRight);
            var bottomResizer = GetTemplateChild<Thumb>("bottom");
            bottomResizer.DragDelta += new DragDeltaEventHandler(ResizeBottom);
            var bottomRightResizer = GetTemplateChild<Thumb>("rightbottom");
            bottomRightResizer.DragDelta += new DragDeltaEventHandler(ResizeBottomRight);
            var topRightResizer = GetTemplateChild<Thumb>("righttop");
            topRightResizer.DragDelta += new DragDeltaEventHandler(ResizeTopRight);
            var topLeftResizer = GetTemplateChild<Thumb>("lefttop");
            topLeftResizer.DragDelta += new DragDeltaEventHandler(ResizeTopLeft);
            var bottomLeftResizer = GetTemplateChild<Thumb>("leftbottom");
            bottomLeftResizer.DragDelta += new DragDeltaEventHandler(ResizeBottomLeft);
        }

        private void TopResizer_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var x = 0;
        }

        private T GetTemplateChild<T>(string childName) where T : FrameworkElement, new()
        {
            return (GetTemplateChild(childName) as T) ?? new T();
        }

        private void ResizeBottomLeft(object sender, DragDeltaEventArgs e)
        {
            ResizeLeft(sender, e);
            ResizeBottom(sender, e);
        }

        private void ResizeTopLeft(object sender, DragDeltaEventArgs e)
        {
            ResizeTop(sender, e);
            ResizeLeft(sender, e);
        }

        private void ResizeTopRight(object sender, DragDeltaEventArgs e)
        {
            ResizeRight(sender, e);
            ResizeTop(sender, e);
        }

        private void ResizeBottomRight(object sender, DragDeltaEventArgs e)
        {
            ResizeBottom(sender, e);
            ResizeRight(sender, e);
        }

        private void ResizeBottom(object sender, DragDeltaEventArgs e)
        {
            if (ActualHeight <= MinHeight && e.VerticalChange < 0)
            {
                return;
            }

            if (double.IsNaN(Height))
            {
                Height = ActualHeight;
            }

            Height += e.VerticalChange;
        }

        private void ResizeRight(object sender, DragDeltaEventArgs e)
        {
            if (ActualWidth <= MinWidth && e.HorizontalChange < 0)
            {
                return;
            }

            if (double.IsNaN(Width))
            {
                Width = ActualWidth;
            }

            Width += e.HorizontalChange;
        }

        private void ResizeLeft(object sender, DragDeltaEventArgs e)
        {
            if (ActualWidth <= MinWidth && e.HorizontalChange > 0)
            {
                return;
            }

            if (double.IsNaN(Width))
            {
                Width = ActualWidth;
            }

            Width -= e.HorizontalChange;
            Left += e.HorizontalChange;
        }

        private void ResizeTop(object sender, DragDeltaEventArgs e)
        {
            if (ActualHeight <= MinHeight && e.VerticalChange > 0)
            {
                return;
            }

            if (double.IsNaN(Height))
            {
                Height = ActualHeight;
            }

            Height -= e.VerticalChange;
            Top += e.VerticalChange;
        }
        #endregion

    }
}
