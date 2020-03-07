using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Controls
{
    /// <summary>
    /// 箭头基类
    /// </summary>
    public abstract class ArrowBase : Shape
    {
        #region Fields

        /// <summary>
        /// 整个形状(包含箭头和具体形状)
        /// </summary>
        private readonly PathGeometry _wholeGeometry = new PathGeometry();

        /// <summary>
        /// 除去箭头外的具体形状
        /// </summary>
        private readonly PathFigure _figureConcrete = new PathFigure();

        /// <summary>
        /// 开始处的箭头线段
        /// </summary>
        private readonly PathFigure _figureStart=new PathFigure();

        /// <summary>
        /// 结束处的箭头线段
        /// </summary>
        private readonly PathFigure _figureEnd=new PathFigure();

        #endregion Fields

        #region Properties

        /// <summary>
        /// 箭头两边夹角的依赖属性
        /// </summary>
        public static readonly DependencyProperty ArrowAngleProperty =
            DependencyProperty.Register("ArrowAngle", typeof(double), typeof(ArrowBase),
                new FrameworkPropertyMetadata(45.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 箭头两边夹角
        /// </summary>
        public double ArrowAngle
        {
            set { SetValue(ArrowAngleProperty, value); }
            get { return (double)GetValue(ArrowAngleProperty); }
        }

        /// <summary>
        /// 箭头长度的依赖属性
        /// </summary>
        public static readonly DependencyProperty ArrowLengthProperty =
            DependencyProperty.Register("ArrowLength", typeof(double), typeof(ArrowBase),
                new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 箭头两边的长度
        /// </summary>
        public double ArrowLength
        {
            set { SetValue(ArrowLengthProperty, value); }
            get { return (double)GetValue(ArrowLengthProperty); }
        }

        /// <summary>
        /// 箭头所在端的依赖属性
        /// </summary>
        public static readonly DependencyProperty ArrowEndsProperty =
            DependencyProperty.Register("ArrowEnds", typeof(ArrowEnds), typeof(ArrowBase),
                new FrameworkPropertyMetadata(ArrowEnds.End, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 箭头所在端
        /// </summary>
        public ArrowEnds ArrowEnds
        {
            set { SetValue(ArrowEndsProperty, value); }
            get { return (ArrowEnds)GetValue(ArrowEndsProperty); }
        }

        /// <summary>
        /// 箭头是否闭合的依赖属性
        /// </summary>
        public static readonly DependencyProperty IsArrowClosedProperty =
            DependencyProperty.Register("IsArrowClosed", typeof(bool), typeof(ArrowBase),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 箭头是否闭合
        /// </summary>
        public bool IsArrowClosed
        {
            set { SetValue(IsArrowClosedProperty, value); }
            get { return (bool)GetValue(IsArrowClosedProperty); }
        }

        /// <summary>
        /// 开始点
        /// </summary>
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
            "StartPoint", typeof(Point), typeof(ArrowBase),
            new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 开始点
        /// </summary>
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        #endregion Properties

        #region Protected Methods

        /// <summary>
        /// 构造函数
        /// </summary>
        protected ArrowBase()
        {
            var polyLineSegStart = new PolyLineSegment();
            _figureStart.Segments.Add(polyLineSegStart);

            var polyLineSegEnd = new PolyLineSegment();
            _figureEnd.Segments.Add(polyLineSegEnd);

            _wholeGeometry.Figures.Add(_figureConcrete);
            _wholeGeometry.Figures.Add(_figureStart);
            _wholeGeometry.Figures.Add(_figureEnd);
        }

        /// <summary>
        /// 获取具体形状的各个组成部分
        /// </summary>
        protected abstract PathSegmentCollection FillFigure();

        /// <summary>
        /// 获取开始箭头处的结束点
        /// </summary>
        /// <returns>开始箭头处的结束点</returns>
        protected abstract Point GetStartArrowEndPoint();

        /// <summary>
        /// 获取结束箭头处的开始点
        /// </summary>
        /// <returns>结束箭头处的开始点</returns>
        protected abstract Point GetEndArrowStartPoint();

        /// <summary>
        /// 获取结束箭头处的结束点
        /// </summary>
        /// <returns>结束箭头处的结束点</returns>
        protected abstract Point GetEndArrowEndPoint();

        /// <summary>
        /// 定义形状
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                _figureConcrete.StartPoint = StartPoint;

                //清空具体形状,避免重复添加
                _figureConcrete.Segments.Clear();
                var segements = FillFigure();
                if (segements != null)
                {
                    foreach (var segement in segements)
                    {
                        _figureConcrete.Segments.Add(segement);
                    }
                }

                //绘制开始处的箭头
                if ((ArrowEnds & ArrowEnds.Start) == ArrowEnds.Start)
                {
                    CalculateArrow(_figureStart, GetStartArrowEndPoint(), StartPoint);
                }

                // 绘制结束处的箭头
                if ((ArrowEnds & ArrowEnds.End) == ArrowEnds.End)
                {
                    CalculateArrow(_figureEnd, GetEndArrowStartPoint(), GetEndArrowEndPoint());
                }

                return _wholeGeometry;
            }
        }

        #endregion  Protected Methods

        #region Private Methods

        /// <summary>
        /// 计算两个点之间的有向箭头
        /// </summary>
        /// <param name="pathfig">箭头所在的形状</param>
        /// <param name="startPoint">开始点</param>
        /// <param name="endPoint">结束点</param>
        /// <returns>计算好的形状</returns>
        private void CalculateArrow(PathFigure pathfig, Point startPoint, Point endPoint)
        {
            var polyseg = pathfig.Segments[0] as PolyLineSegment;
            if (polyseg != null)
            {
                var matx = new Matrix();
                Vector vect = startPoint - endPoint;
                //获取单位向量
                vect.Normalize();
                vect *= ArrowLength;
                //旋转夹角的一半
                matx.Rotate(ArrowAngle / 2);
                //计算上半段箭头的点
                pathfig.StartPoint = endPoint + vect * matx;

                polyseg.Points.Clear();
                polyseg.Points.Add(endPoint);

                matx.Rotate(-ArrowAngle);
                //计算下半段箭头的点
                polyseg.Points.Add(endPoint + vect * matx);
            }

            pathfig.IsClosed = IsArrowClosed;
        }

        #endregion Private Methods

    }
}
