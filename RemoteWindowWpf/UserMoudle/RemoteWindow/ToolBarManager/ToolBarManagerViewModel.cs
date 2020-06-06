using Controls.Dialogs;
using Dispath;
using LYL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserMoudle.RemoteWindow.remoteControlLogic;
using UserMoudle.RemoteWindow.ToolBarManager;

namespace UserMoudle.RemoteWindow.FileSendManager
{
    public class ToolBarManagerViewModel : ViewModelBase
    {
        private int _currentFileCount;
        public int CurrentFileCount
        {
            get { return _currentFileCount; }
            set
            {
                _currentFileCount = value;
                OnPropertyChanged();
            }
        }
        RemoteControlBase controlContext { get; set; }
        public FileList FileListView = null;

        public void setControlContext(RemoteControlBase controlContext_)
        {
            this.controlContext = controlContext_;
            FileChannelMessageDeal.onFileCountChanged -= FileChannelMessageDeal_onFileCountChanged;
            FileChannelMessageDeal.onFileCountChanged += FileChannelMessageDeal_onFileCountChanged;
            FileChannelMessageDeal.onFragChanged -= FileChannelMessageDeal_onFragChanged;
            FileChannelMessageDeal.onFragChanged += FileChannelMessageDeal_onFragChanged;
        }

        ~ToolBarManagerViewModel()
        {
            FileChannelMessageDeal.onFileCountChanged -= FileChannelMessageDeal_onFileCountChanged;
            FileChannelMessageDeal.onFragChanged -= FileChannelMessageDeal_onFragChanged;
        }

        private void FileChannelMessageDeal_onFileCountChanged(object sender, int e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.CurrentFileCount = e;
                if (this.FileListView == null)
                {
                    return;
                }
                this.FileListView.FileCountRefresh();
            }));
        }

        private void FileChannelMessageDeal_onFragChanged(object sender, FileFragInfo e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.FileListView == null)
                {
                    return;
                }
                this.FileListView.FileFragRefresh(e);
            }));
        }

        #region 发送文件
        public DelegateCommand startSendFileCommand => new DelegateCommand(this.startSendFile);
        private void startSendFile(object obj)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.controlContext == null || this.controlContext.fileChannel == null)
                {
                    return;
                }
                FileChannelMessageDeal.SelectFileToSend(this.controlContext.fileChannel);
            }));
        }


        public DelegateCommand showFileListCommand => new DelegateCommand(this.showFileList);
        private void showFileList(object obj)
        {
            string dialogHostIden = obj.ToString();
            this.FileListView = null;
            this.FileListView = new FileList();
            this.FileListView.Width = 400;
            this.FileListView.Height = 400;
            this.FileListView.PopupDialog(dialogHostIden);
            this.FileListView.FileCountRefresh();
        }



        #endregion

    }
}
