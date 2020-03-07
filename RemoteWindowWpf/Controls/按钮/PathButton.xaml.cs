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
    /// PathButton.xaml 的交互逻辑
    /// </summary>
    public partial class PathButton : Border
    {
        public PathButton()
        {
            InitializeComponent();
        }


        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(PathButton), new PropertyMetadata(null, newpath));

        private static void newpath(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathButton btn = d as PathButton;
            btn.pathbutton.Data = Geometry.Parse(e.NewValue as string);
        }

        /// <summary>
        ///// ////////////////////////////////////////////////////////////////
        /// </summary>
        public Brush ForeGround
        {
            get { return (Brush)GetValue(ForeGroundProperty); }
            set { SetValue(ForeGroundProperty, value); }
        }

        public static readonly DependencyProperty ForeGroundProperty =
            DependencyProperty.Register("ForeGround", typeof(Brush), typeof(PathButton), new PropertyMetadata(null, newforeground));

        private static void newforeground(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathButton btn = d as PathButton;
            btn.pathbutton.Fill = e.NewValue as Brush;
            btn.text1.Foreground = e.NewValue as Brush;
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PathButton), new PropertyMetadata(null,newtext));

        private static void newtext(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathButton btn = d as PathButton;
            btn.text1.Text = e.NewValue as string;
            btn.ToolTip = e.NewValue as string;
        }
        ///////////////////////////////////////////////////////////////////
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(PathButton), new PropertyMetadata(null));
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////
        public object CommandPamar
        {
            get { return (object)GetValue(CommandPamarProperty); }
            set { SetValue(CommandPamarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandPamarProperty =
            DependencyProperty.Register("CommandPamar", typeof(object), typeof(PathButton), new PropertyMetadata(null));


        private void pathButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
             if(Command!=null)
            {
                Command.Execute(CommandPamar);
            }
        }
    }
}
