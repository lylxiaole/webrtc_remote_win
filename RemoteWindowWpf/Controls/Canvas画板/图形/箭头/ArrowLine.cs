using System.Windows;
using System.Windows.Media;

namespace Controls
{
    /// <summary>
    /// 两点之间带箭头的直线
    /// </summary>
    public class ArrowLine:ArrowBase
    {
        #region Fields

        /// <summary>
        /// 线段
        /// </summary>
        private readonly LineSegment _lineSegment=new LineSegment();

        #endregion Fields

        #region Properties

        /// <summary>
        /// 结束点
        /// </summary>
        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
            "EndPoint", typeof(Point), typeof(ArrowLine), 
            new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 结束点
        /// </summary>
        public Point EndPoint
        {
            get { return (Point) GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        #endregion Properties

        #region Protected Methods

        /// <summary>
        /// 填充Figure
        /// </summary>
        protected override PathSegmentCollection FillFigure()
        {
            _lineSegment.Point = EndPoint;
            return new PathSegmentCollection
            {
                _lineSegment
            };
        }

        /// <summary>
        /// 获取开始箭头处的结束点
        /// </summary>
        /// <returns>开始箭头处的结束点</returns>
        protected override Point GetStartArrowEndPoint()
        {
            return EndPoint;
        }

        /// <summary>
        /// 获取结束箭头处的开始点
        /// </summary>
        /// <returns>结束箭头处的开始点</returns>
        protected override Point GetEndArrowStartPoint()
        {
            return StartPoint;
        }

        /// <summary>
        /// 获取结束箭头处的结束点
        /// </summary>
        /// <returns>结束箭头处的结束点</returns>
        protected override Point GetEndArrowEndPoint()
        {
            return EndPoint;
        }

        #endregion  Protected Methods
    }
}
