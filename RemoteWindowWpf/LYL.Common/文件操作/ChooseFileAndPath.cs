using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LYL.Common
{
    public struct Params
    {
        public const int flagSize = 1024 * 50;
    }
    public class ChooseFileAndPath
    {
        private static string imageFilter = "图片文件(*.*)|*.jpeg|JPG|*.jpg|GIF|*.gif|PNG|*.png";
        private static string imageFilterall = "选择文件(*.*)|*.*";

        private static string selectPath()
        {
            string path = string.Empty;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = imageFilterall//如果需要筛选txt文件（"Files (*.txt)|*.txt"）
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                path = openFileDialog.FileName;
            }
            return path;
        }

        /// <summary>
        /// 选择文件，分析切片信息
        /// </summary>
        /// <returns></returns>
        public static FileFragInfo selectFile()
        {
            var path = selectPath();
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var flag = new FileFragInfo();

            using (var fstrem = File.OpenRead(path))
            {
                var fileinfo = new FileInfo(path);
                //
                flag.fileSize = fstrem.Length;
                if (fstrem.Length % Params.flagSize == 0)
                {
                    flag.fragLength = (int)fstrem.Length / Params.flagSize;
                }
                else
                {
                    flag.fragLength = (int)(fstrem.Length / Params.flagSize) + 1;
                }
                flag.fileExt = fileinfo.Extension;
                flag.fileName = fileinfo.Name;
                flag.filePath = fileinfo.FullName;
                flag.fileGuid = Guid.NewGuid().ToString();
            }
            return flag;
        }

    }
    public class FileFragInfo
    {
        public string fileGuid { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string fileExt { get; set; }
        /// <summary>
        /// bytes长度
        /// </summary>
        public long fileSize { get; set; }
        public int fragLength { get; set; }
        public int currentFragIndex { get; set; }
    }

    public class FileFrag
    {
        public string fileGuid { get; set; }
        public int currentFragIndex { get; set; } = 0;

        public int bytesLength { get; set; }
        public byte[] bytes { get; set; }
    }

    public class AsyncState
    {
        public FileStream FS { get; set; }
        public FileFragInfo fileFragInfo { get; set; }

        public FileFrag currentFrag { get; set; }
    }
    public class FileFragHelper_Read
    {
        public FileFragInfo CurrentFileInfo
        {
            get
            {
                if (this.CurrentAsyncState == null)
                {
                    return null;
                }
                return this.CurrentAsyncState.fileFragInfo;
            }
        }

        public FileFrag CurrentFrag
        {
            get
            {
                if (this.CurrentAsyncState == null)
                {
                    return null;
                }
                return this.CurrentAsyncState.currentFrag;
            }
        }
 
        private AsyncState CurrentAsyncState { get; set; }
        public bool SelectFileToFragSend()
        {
            var fraginfo = ChooseFileAndPath.selectFile();
            if (fraginfo == null)
            {
                return false;
            }
            var fileinfo = File.OpenRead(fraginfo.filePath);
            var frag = new FileFrag
            {
                currentFragIndex = 0,
                fileGuid = fraginfo.fileGuid,
                bytes = new byte[Params.flagSize]
            };
            //构造BeginRead需要传递的状态
            this.CurrentAsyncState = new AsyncState { FS = fileinfo, currentFrag = frag, fileFragInfo = fraginfo }; 
            return true; 
        }

        private void AsyncReadCallback(IAsyncResult asyncResult)
        {
            this.CurrentAsyncState = (AsyncState)asyncResult.AsyncState;
            try
            {
                int readCn = this.CurrentAsyncState.FS.EndRead(asyncResult);
                //判断是否读到内容
                if (readCn > 0)
                {
                    this.CurrentAsyncState.currentFrag.bytesLength = readCn;
                    var newbuffer = this.CurrentAsyncState.currentFrag.bytes.Take(readCn).ToArray();
                    this.CurrentAsyncState.currentFrag.bytes = newbuffer;
                    //输出读取内容值
                    this.onReadFrag?.Invoke(this, this);
                }
                //
                if (readCn < Params.flagSize)
                {
                    this.onEndReadFrag?.Invoke(this, this);
                    this.CurrentAsyncState.FS.Close();
                    this.CurrentAsyncState.FS.Dispose();
                } 
            }
            catch (Exception ex)
            {
                this.onReadFragError?.Invoke(this, this);
            }
        }


        public void StartRead()
        {
            //构造BeginRead需要传递的状态 
            this.CurrentAsyncState.FS.BeginRead(this.CurrentAsyncState.currentFrag.bytes, 0, Params.flagSize, new AsyncCallback(AsyncReadCallback), this.CurrentAsyncState);
        }

        public void ContinueRead()
        {
            this.CurrentAsyncState.currentFrag.currentFragIndex++;
            Array.Clear(this.CurrentAsyncState.currentFrag.bytes, 0, Params.flagSize);
            //再次执行异步读取操作
            this.CurrentAsyncState.FS.BeginRead(this.CurrentAsyncState.currentFrag.bytes, 0, Params.flagSize, new AsyncCallback(AsyncReadCallback), this.CurrentAsyncState);
        }


        public void Dispose()
        {
            if (this.CurrentAsyncState == null)
            {
                return;
            }

            try
            {
                this.CurrentAsyncState.FS.Close();
                this.CurrentAsyncState.FS.Dispose();
            }
            catch (Exception)
            {
            }
        }
         
        public event EventHandler<FileFragHelper_Read> onReadFrag;
        public event EventHandler<FileFragHelper_Read> onEndReadFrag;
        public event EventHandler<FileFragHelper_Read> onReadFragError;
    }

}
