using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UserMoudle.RemoteWindow.remoteControlLogic
{
    public class VideoFrameDeal
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap">格式为ARGB</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static WriteableBitmap dealImageByte(byte[] bitmap, int width, int height)
        {
            WriteableBitmap wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            //argb转换成rgba
            //for (var v = 0; v < bitmap.Length; v += 4)
            //{
            //    var a = bitmap[v];
            //    bitmap[v] = bitmap[v + 1];
            //    bitmap[v + 1] = bitmap[v + 2];
            //    bitmap[v + 2] = bitmap[v + 3];
            //    bitmap[v + 3] = a;
            //}
            int stride = (wb.PixelWidth * wb.Format.BitsPerPixel) / 8;
            wb.WritePixels(rect, bitmap, stride, 0);
            wb.Freeze();
            return wb; 
        } 
    }
}
