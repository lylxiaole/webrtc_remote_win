using Controls.Dialogs;
using Dispath;
using LYL.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserMoudle.RemoteWindow.remoteControlLogic;

namespace UserMoudle.RemoteWindow.ToolBarManager
{
    /// <summary>
    /// FileList.xaml 的交互逻辑
    /// </summary>
    public partial class FileList : DialogBase
    {
        public FileList()
        {
            InitializeComponent();
            this.DataContext = new FileListViewModel();
        }

        public void FileCountRefresh()
        {
            Application.Current.Dispatcher.Invoke(() =>
           {
               this.CurrentContext.FileCountRefresh();
           });
        }

        public void FileFragRefresh(FileFragInfo fileinfo)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.CurrentContext.FileFragRefresh(fileinfo);
            });
        }

        private FileListViewModel CurrentContext
        {
            get
            {
                return this.DataContext as FileListViewModel;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileChannelMessageDeal.OpenFileDiction();
        }
    }

    public class FragState : ViewModelBase
    {
        public string _persentStr;
        public string PersentStr
        {
            get
            {
                return _persentStr;
            }
            set
            {
                _persentStr = value;
                OnPropertyChanged();
            }
        }

        public string _fileLocalName;
        public string FileLocalName
        {
            get
            {
                return _fileLocalName;
            }
            set
            {
                _fileLocalName = value;
                OnPropertyChanged();
            }
        }

        public string _fileGuid;
        public string FileGuid
        {
            get
            {
                return _fileGuid;
            }
            set
            {
                _fileGuid = value;
                OnPropertyChanged();
            }
        }
    }
    public class FileListViewModel : ViewModelBase
    {
        public ObservableCollection<FragState> _currentSendFiles = new ObservableCollection<FragState>();
        public ObservableCollection<FragState> CurrentSendFiles
        {
            get
            {
                return _currentSendFiles;
            }
            set
            {
                this._currentSendFiles = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FragState> _currentRecivesFiles = new ObservableCollection<FragState>();
        public ObservableCollection<FragState> CurrentRecivesFiles
        {
            get
            {
                return _currentRecivesFiles;
            }
            set
            {
                this._currentRecivesFiles = value;
                OnPropertyChanged();
            }
        }

        private string cuteFilePercent(FileFragInfo fileinfo)
        {
            return ((double)fileinfo.currentFragIndex * 100 / fileinfo.fragLength).ToString("F2") + "%";
        }


        public void FileCountRefresh()
        {
            var sendingFiles = FileChannelMessageDeal.GetAllSendingFileInfo();
            var recivingFiles = FileChannelMessageDeal.GetAllRecivingingFileInfo();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                CurrentSendFiles.Clear();
                CurrentRecivesFiles.Clear();

                foreach (var item in sendingFiles)
                {
                    CurrentSendFiles.Add(
                        new FragState
                        {
                            FileGuid = item.fileGuid,
                            FileLocalName = item.fileName,
                            PersentStr = cuteFilePercent(item)
                        });
                }

                foreach (var item in recivingFiles)
                {
                    CurrentRecivesFiles.Add(
                      new FragState
                      {
                          FileGuid = item.fileGuid,
                          FileLocalName = item.fileGuid + "__" + item.fileName,
                          PersentStr = cuteFilePercent(item)
                      });
                }
            }));
        }

        public void FileFragRefresh(FileFragInfo fileinfo)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var find = CurrentSendFiles.FirstOrDefault(o => o.FileGuid == fileinfo.fileGuid);
                if (find != null)
                {
                    find.PersentStr = cuteFilePercent(fileinfo);
                    //CurrentSendFiles = CurrentSendFiles;
                }
                find = CurrentRecivesFiles.FirstOrDefault(o => o.FileGuid == fileinfo.fileGuid);
                if (find != null)
                {
                    find.PersentStr = cuteFilePercent(fileinfo);
                    //CurrentSendFiles = CurrentSendFiles;
                }
            }));
        }
    }
}
