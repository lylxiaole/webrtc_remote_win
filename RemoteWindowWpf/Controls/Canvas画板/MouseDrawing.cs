using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Controls
{
    //鼠标画画类
    public class MouseDrawing : DrawingCanvasBase
    {
        public MouseDrawing()
        {

            this.MouseDown += MouseDrawing_MouseDown;
            this.MouseUp += MouseDrawing_MouseUp;
            this.MouseMove += MouseDrawing_MouseMove;
            this.Drop += MouseDrawing_Drop;
            this.PreviewKeyDown += MouseDrawing_PreviewKeyDown;
            this.Focusable = true;
            this.Focus();
            this.AllowDrop = true;

        }

        private void MouseDrawing_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (visual != null)
                {
                    this.RemoveVisual(visual);
                    if (biankuang != null)
                    {
                        this.RemoveVisual(biankuang);
                    }
                }

                if (ArrowVisual != null)
                {
                    this.RemoveVisual(ArrowVisual);
                }
            }

            else if (e.Key == Key.S && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Bitmap files (*.bmp)|*.bmp ";
                if ((bool)dlg.ShowDialog())
                {
                    try
                    {
                        using (FileStream file = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
                        {
                            int marg = 12;
                            RenderTargetBitmap rtb = new RenderTargetBitmap((int)this.ActualWidth * marg, (int)this.ActualHeight * marg, 96d * marg, 96d * marg, PixelFormats.Pbgra32);
                            rtb.Render(this);
                            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(rtb));
                            encoder.Save(file);
                            file.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }
            }
        }



        #region 在画板中拖入图片
        //拖拽图片进入该控件,并显示出来
        private void MouseDrawing_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                foreach (var v in files)
                {
                    var image = InsertImage(v, new Rect(10, 10, 100, 100));
                }
            }
        }
        //删除bitmap所占的内存
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// 将图片转换成ImageSource
        /// </summary>
        /// <param name="bitmapPath"></param>
        /// <returns></returns>
        public ImageSource GetImageSourceFromPath(string bitmapPath)
        {
            try
            {
                Bitmap bitmap = new Bitmap(bitmapPath);
                IntPtr hBitmap = bitmap.GetHbitmap();
                ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                if (!DeleteObject(hBitmap))
                {
                    throw new System.ComponentModel.Win32Exception();
                }
                return wpfBitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 添加一个图片
        /// </summary>
        /// <param name="imagepath"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        protected Visual InsertImage(string imagepath, Rect rect)
        {
            var imagesource = GetImageSourceFromPath(imagepath);
            if (imagesource == null)
            {
                return null;
            }
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();

            dc.DrawImage(imagesource, rect);
            dc.Close();
            this.AddVisual(visual);
            return visual;
        }
        #endregion


        #region 通过鼠标移动画箭头
        System.Windows.Point lastPoint = new System.Windows.Point();
        //箭头
        Visual ArrowVisual = null;
        private void MouseDrawing_MouseMove(object sender, MouseEventArgs e)
        {
            var lastPoint = e.GetPosition(this);
            if (visual != null && e.LeftButton == MouseButtonState.Pressed)
            {
                visual.Offset += (new Vector(lastPoint.X - startPoint.X, lastPoint.Y - startPoint.Y));
                biankuang.Offset += (new Vector(lastPoint.X - startPoint.X, lastPoint.Y - startPoint.Y));
                startPoint = lastPoint;
            }
            else if (visual == null && e.LeftButton == MouseButtonState.Pressed)//开始画箭头
            {
                if (Math.Abs(lastPoint.X - startPoint.X) > 8 && Math.Abs(lastPoint.Y - startPoint.Y) > 8)
                {
                    if (ArrowVisual != null)
                    {
                        this.RemoveVisual(ArrowVisual);
                    }
                    ArrowVisual = DrawArrow(startPoint, lastPoint, System.Windows.Media.Brushes.Blue, 3);
                }
            }
        }

        System.Windows.Point startPoint = new System.Windows.Point();
        //图片边框
        DrawingVisual biankuang;
        DrawingVisual visual;
        private void MouseDrawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;

            #region 命中测试
            if (biankuang != null)
            {
                this.RemoveVisual(biankuang);
            }
            startPoint = e.GetPosition(this);
            visual = this.GetVisual(startPoint);
            //选中内容块，并对块进行命中测试,为块画上边框
            if (visual != null)
            {
                DrawLine(visual);
            }

            #endregion
        }

        private void MouseDrawing_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            if (startPoint == null)
            {
                return;
            }

            ArrowVisual = null;
        }

        //画箭头
        protected Visual DrawArrow(System.Windows.Point startpoint, System.Windows.Point endpoint, System.Windows.Media.Brush brush, double thick)
        {
            var matx = new Matrix();
            Vector vect = startPoint - endpoint;
            //获取单位向量
            vect.Normalize();
            vect *= 15;
            //旋转夹角的一半
            matx.Rotate(60 / 2);
            //计算上半段箭头的点
            System.Windows.Point point1 = endpoint + vect * matx;
            matx.Rotate(-60);
            //计算下半段箭头的点
            System.Windows.Point point2 = endpoint + vect * matx;
            DrawingVisual visual = new DrawingVisual();
            System.Windows.Media.Pen pen = new System.Windows.Media.Pen(brush, thick);
            using (var dc = visual.RenderOpen())
            {
                dc.DrawLine(pen, startPoint, endpoint);
                dc.DrawLine(pen, endpoint, point1);
                dc.DrawLine(pen, endpoint, point2);
                pen.Freeze();  //冻结画笔，这样能加快绘图速度
            }

            this.AddVisual(visual);
            return visual;
        }


        //为图片画一个正方形边框
        private void DrawLine(DrawingVisual visual)
        {
            DrawingVisual newvis = new DrawingVisual();
            biankuang = newvis;

            var visualwidth = (visual.ContentBounds.TopRight - visual.ContentBounds.TopLeft).X;
            var visualheight = (visual.ContentBounds.BottomLeft - visual.ContentBounds.TopLeft).Y;
            var topleft = new System.Windows.Point(visual.Offset.X + visual.ContentBounds.TopLeft.X, visual.Offset.Y + visual.ContentBounds.TopLeft.Y);
            var topright = new System.Windows.Point(visual.Offset.X + visual.ContentBounds.TopLeft.X + visualwidth, visual.Offset.Y + visual.ContentBounds.TopLeft.Y);
            var bottomleft = new System.Windows.Point(visual.Offset.X + visual.ContentBounds.TopLeft.X, visual.Offset.Y + visualheight + visual.ContentBounds.TopLeft.Y);
            var bottomright = new System.Windows.Point(visual.Offset.X + visualwidth + visual.ContentBounds.TopLeft.X, visual.Offset.Y + visualheight + visual.ContentBounds.TopLeft.Y);

            using (var dc = newvis.RenderOpen())
            {
                System.Windows.Media.Brush brush = System.Windows.Media.Brushes.Blue;
                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Blue, 2);
                dc.DrawLine(pen, topleft, topright);
                dc.DrawLine(pen, topleft, bottomleft);
                dc.DrawLine(pen, topright, bottomright);
                dc.DrawLine(pen, bottomleft, bottomright);
                pen.Freeze();
            }
            this.AddVisual(newvis);
        }
        #endregion




        /// <summary>
        /// 析构
        /// </summary>
        ~MouseDrawing()
        {
            //foreach (var v in this.visuals)
            //{
            //    RemoveVisual(v);
            //}
            //this.visuals.Clear();
        }
    }
}
