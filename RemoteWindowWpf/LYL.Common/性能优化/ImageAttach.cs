using Dispath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LYL.Common
{
    public class ImageAttach
    {
        static Dictionary<int, BitmapImage> allImages = new Dictionary<int, BitmapImage>();
        static Dictionary<int, int> imageUseCount = new Dictionary<int, int>();
        #region 附加属性
        public static object GetSource(DependencyObject obj)
        {
            return (object)obj.GetValue(SourceProperty);
        }
        public static void SetSource(DependencyObject obj, object value)
        {
            obj.SetValue(SourceProperty, value);
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(object), typeof(ImageAttach), new PropertyMetadata(null, ImageSourceChanged));

        public static int GetDecodeWidth(DependencyObject obj)
        {
            return (int)obj.GetValue(DecodeWidthProperty);
        }
        public static void SetDecodeWidth(DependencyObject obj, int value)
        {
            obj.SetValue(DecodeWidthProperty, value);
        }
        public static readonly DependencyProperty DecodeWidthProperty =
            DependencyProperty.RegisterAttached("DecodeWidth", typeof(int), typeof(ImageAttach), new PropertyMetadata(0));


        public static int GetDecodeHeight(DependencyObject obj)
        {
            return (int)obj.GetValue(DecodeHeightProperty);
        }
        public static void SetDecodeHeight(DependencyObject obj, int value)
        {
            obj.SetValue(DecodeHeightProperty, value);
        }
        public static readonly DependencyProperty DecodeHeightProperty =
            DependencyProperty.RegisterAttached("DecodeHeight", typeof(int), typeof(ImageAttach), new PropertyMetadata(0));



        #endregion

        private static void ImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = d as Image;
            if (d == null) { return; }
            var imagesource = e.NewValue;
            makeImageSource(image, imagesource);
            image.Unloaded -= Image_Unloaded;
            image.Unloaded += Image_Unloaded;
        }

        private static void Image_Unloaded(object sender, RoutedEventArgs e)
        {
           var imgcon= sender as Image;
            RemoveRecord(imgcon.GetHashCode());
        }

        private static void makeImageSource(Image imageCon, object source)
        {
            if (string.IsNullOrEmpty(source.ToString()))
            {
                RemoveRecord(imageCon.GetHashCode());
                imageCon.Source = null;
                return;
            }
            var bitmap = source as byte[];
            var imgpath = source.ToString();
            if(bitmap==null||string.IsNullOrEmpty(imgpath))
            {
                throw new Exception("图片资源类型只能是本地路径或者Byte数组");
            }
             
            var imageHash = (bitmap == null ? imgpath.GetHashCode() : bitmap.GetHashCode());

            var sameImg = GetSameImage(imageHash);
            if (sameImg != null)
            {
                imageCon.Source = sameImg; 
                AddRecord(imageHash, imageCon.GetHashCode());
                return;
            }

            BitmapImage strongBitmap = null;
            if (bitmap != null)
            {
                strongBitmap = dealImageByte(imageCon, bitmap);
            }

            if (string.IsNullOrEmpty(imgpath) == false)
            {
                strongBitmap = dealImagePath(imageCon, imgpath);
            }

            if (strongBitmap != null)
            {
                allImages.Add(imageHash, strongBitmap);
                AddRecord(imageHash, imageCon.GetHashCode());
                imageCon.Source = strongBitmap.SetWeak(); 
            }
        }

        private static BitmapImage GetSameImage(int imageHash)
        {
            if (allImages.ContainsKey(imageHash) == false)
            {
                return null;
            }
            else
            {
                return allImages[imageHash].SetWeak();
            }
        }

        private static BitmapImage dealImageByte(Image imgCon, byte[] bitmap)
        { 
            var ibw = bitmap.SetWeak();
            using (MemoryStream ms = new MemoryStream(bitmap))
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.DecodePixelWidth = GetDecodeWidth(imgCon);
                img.DecodePixelHeight = GetDecodeHeight(imgCon);
                img.StreamSource = ms;
                img.CacheOption = BitmapCacheOption.OnLoad; 
                img.EndInit();
                bitmap = null;
                return img;
            }
          
        }

        private static BitmapImage dealImagePath(Image imgCon, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs.Dispose();
            return dealImageByte(imgCon, buffer);
        }
        #region 引用计数
        private static void RemoveRecord(int imageConHash)
        {
            if(imageUseCount.ContainsKey(imageConHash)==false)
            {
                return;
            }
            var imageHash = imageUseCount[imageConHash];
            var imageHashCount = imageUseCount.Values.Where(o=>o==imageHash).Count();
            if(imageHashCount>1)
            {
                imageUseCount.Remove(imageConHash);
                return;
            }

            if(allImages.ContainsKey(imageHash)==true)
            {
                var bitmap = allImages[imageHash];
                allImages.Remove(imageHash);
                bitmap.StreamSource.Dispose();
                bitmap = null; 
            }
            imageUseCount.Remove(imageConHash);
        }

        private static void AddRecord(int imageHash, int imageConHash)
        {
            if (imageUseCount.ContainsKey(imageConHash) == false)
            {
                imageUseCount.Add(imageConHash,imageHash);
                return;
            }

            var oldimageHash = imageUseCount[imageConHash];

            if(oldimageHash!=imageHash)
            { 
                RemoveRecord(imageConHash);
            }
        }
        #endregion
    }
}
