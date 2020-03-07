
using Dispath;
using LYL.Common;
using LYL.Logic;
using LYL.Logic.Machine;
using MaterialDesignThemes.Wpf;
using MouseKeyPlayback;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UserMoudle.RemoteWindow.FileSendManager; 
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.RemoteWindow
{
    public class ControllerWindowViewModel : ViewModelBase
    {
        public event EventHandler onClose;
        public ObservableCollection<RemoteController> controllers { get; set; } = new ObservableCollection<RemoteController>();

        private RemoteController _currentController;
        public RemoteController currentController
        {
            get
            {
                if (_currentController == null)
                {
                    _currentController = this.controllers.FirstOrDefault();
                }
                return _currentController;
            }
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _currentController = value;
                    foreach (var v in this.controllers)
                    {
                        v.RemoteImageControl = null;
                    }
                    if (value != null)
                    {
                        value.RemoteImageControl = this.RemoteImageControl;
                    }
                    this.toolbarManager.setControlContext(_currentController);
                    this.OnPropertyChanged();
                });
            }
        } 
        public ToolBarManagerViewModel toolbarManager { get; set; } = new ToolBarManagerViewModel();
         
        public RemoteController findRemoteController(string remoteId)
        {

            return controllers.FirstOrDefault(o => o.remoteMachine.machineId == remoteId);
        }
         
        public Image RemoteImageControl { get; set; }

        public ControllerWindowViewModel( )
        { 
        }

        #region sdp和ice信令处理 
        /// <summary>
        /// 主动呼叫
        /// </summary>
        /// <param name="mmachineId"></param>
        public void ConnectRemote(string machineId)
        {
            var newcontrol = this.findRemoteController(machineId);
            if (newcontrol == null)
            {
                newcontrol = new RemoteController( ); 
                this.OnPropertyChanged(nameof(this.controllers)); 
                newcontrol.onCloseEvent += RemoteController_onCloseEvent; 
                newcontrol.ConnectRemote(machineId);
                this.controllers.Add(newcontrol);
                this.OnPropertyChanged(nameof(controllers));
            }
            this.currentController = newcontrol;
        }

        private void RemoteController_onCloseEvent(object sender, Guid id)
        {
            (sender as RemoteController).CloseConnection();
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.removeController(sender as RemoteController);
            });
        }

        /// <summary>
        /// 主动连接对方成功，接受对方发来的answer，完成sdp交换
        /// </summary>
        /// <param name="mchineId"></param>
        /// <param name="offerinfo"></param>
        public void SetRemoteAnswer(string machineId, string sdp, string type)
        {
            var newcontrol = this.findRemoteController(machineId);
            newcontrol?.SetRemoteAnswer(machineId, sdp, type);
        }

        public void onRemoteClientSdpCompleted(string machineId)
        {
            var newcontrol = this.findRemoteController(machineId);
            newcontrol?.onRemoteClientSdpCompleted();
        }

        /// <summary>
        /// 接收到对方的Candidate，进行处理
        /// </summary>
        /// <param name="candidate"></param> 
        public void AddRemoteIceCandite(string machineId, IceCandidate iceCandidate)
        {
            var newcontrol = this.findRemoteController(machineId);
            newcontrol?.AddRemoteCandidate(machineId, iceCandidate);
        }


        public List<Record> eventsToSend { get; set; } = new List<Record>();
        //public void SendWinEvents(List<Record> events)
        //{
        //    if (this.currentController == null)
        //    {
        //        return;
        //    }
        //    this.currentController.SendWinApiEvents(events);
        //} 
        public void SendWinEvent(Record revent)
        { 
            if (this.currentController == null)
            {
                return;
            }
            eventsToSend.Clear();
            eventsToSend.Add(revent);
            this.currentController.SendWinApiEvents(eventsToSend);
        }
        #endregion

        #region 动作处理
        public DelegateCommand closeConnectionCommand => new DelegateCommand(this.closeConnection);
        private void closeConnection(object obj)
        {
            string remoteId = obj.ToString();
            //this.wsclient.SendMessage(remoteId, null, msgType.client_onCaller_CloseRemote);
            //
            var remoteController = this.findRemoteController(remoteId);
            if (remoteController != null)
            {
                remoteController.CloseConnection();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.removeController(remoteController);
                });
            }
        }
        private void removeController(RemoteController controller)
        {
            if (this.controllers.IndexOf(controller) < 0)
            {
                return;
            }
            this.controllers.Remove(controller);
            if (controller == this.currentController)
            {
                this.currentController = null;
            }

            if (controllers.Count < 1)
            {
                this.onClose?.Invoke(this, null);
            }
            if (controllers.Count > 0)
            {
                this.currentController = this.controllers[0];
            }
            this.OnPropertyChanged(nameof(controllers));
        }
        public void CloseAllConnetions()
        {
            foreach (var v in this.controllers)
            {
                v.CloseConnection();
            }
        }
       
        #endregion

        ~ControllerWindowViewModel()
        {
            this.CloseAllConnetions();
        }

    }
}
