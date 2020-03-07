using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Controls
{
    //基类，只允许被继承，不允许被直接调用
    public abstract class DrawingCanvasBase : Canvas
    {
        protected List<Visual> visuals = new List<Visual>();
        //这个必须这样重写
        protected override int VisualChildrenCount { get { return visuals.Count; } }
        //这个必须这样重写
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        //添加Visual
        protected void AddVisual(Visual visual)
        {
            visuals.Add(visual);
            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }
        //删除Visual
        protected void RemoveVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }
        //命中测试
        protected DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult?.VisualHit as DrawingVisual;
        }
        //使用DrawVisual画Polyline
        protected Visual Polyline(PointCollection points, Brush color, double thinkness)
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();
            Pen pen = new Pen(color, 3);
            pen.Freeze();  //冻结画笔，这样能加快绘图速度

            for (int i = 0; i < points.Count - 1; i++)
            {
                dc.DrawLine(pen, points[i], points[i + 1]);
            }

            dc.Close();
            return visual;
        }
    }


}
