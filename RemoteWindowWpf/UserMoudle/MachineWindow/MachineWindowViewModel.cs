using Controls.Dialogs;
using Dispath;
using Dispath.MoudleInterface;
using LYL.Common;
using LYL.Data.Models;
using LYL.Logic.Machine;
using LYL.Logic.Types;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UserMoudle.RemoteWindow;
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.MachineWindow
{
    public class MachineWindowViewModel : ViewModelBase
    {
        BeControllerLogic BeControllerLogic { get; set; } = new BeControllerLogic();
        ControllerLogic ControllerLogic { get; set; } = new ControllerLogic();
        WebSocketClient wsclient { get; set; } = new WebSocketClient();
        public MachineInfo CurrentMachine
        {
            get
            {
                var machineInfo = MachineLogic.localMachine();
                return machineInfo;
            }
        }
        public ObservableCollection<LYLUserMachineInfo> myMachines { get; set; } = new ObservableCollection<LYLUserMachineInfo>();

        public MachineWindowViewModel()
        {
            this.wsclient.OnMessage += WebSocketClient_OnMessage;
            this.wsclient.OnClose += Wsclient_OnClose;
            this.wsclient.OnError += Wsclient_OnError;
            this.wsclient.OnOpen += Wsclient_OnOpen;
            this.wsclient.StartClient();
        }
        #region websocket链接
        LoadingDialog waitingCon;
        private void Wsclient_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
        }
        private void Wsclient_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            Task.Run(() =>
            {
                this.showWaiting();
                while (wsclient.state != WebSocketSharp.WebSocketState.Open)
                {
                    wsclient.StartClient();
                    Thread.Sleep(30000);
                }
            });
        }
        private void Wsclient_OnOpen(object sender, EventArgs e)
        {
            this.closeWaiting();
        }

        private void WebSocketClient_OnMessage(object sender, websocketMsgTemp<object> e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.wsclient.DealMessage(this, e);
            }));
        }

        private void showWaiting()
        {
            if (waitingCon != null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                waitingCon = new LoadingDialog();
                waitingCon.PopupDialog(dialogIdentifier: "dskjfdsjfdslkjfsldkfj", closingEventHandler: (a, b) =>
                {
                    waitingCon = null; 
                    Environment.Exit(0);

                });
            });
        }
        private void closeWaiting()
        {
            if (waitingCon == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                waitingCon.CloseDialog();
                waitingCon = null;
            });
        }
        #endregion

        #region websocket消息处理

        [MessageType(msgType.client_onRequestConnect)]
        public void onRecive_RequestConnect(websocketMsgTemp<object> e)
        {
            string remoteMachineId = e.sendMachineId;
            string machinePwd = e.content.ToString();


            if (machinePwd == this.CurrentMachine.machinepwd && this.BeControllerLogic.CanConnect == true)
            {
                WebSocketClient.SendMessage(remoteMachineId, true, msgType.client_onAnswerRequestConnect);
            }
            else
            {
                WebSocketClient.SendMessage(remoteMachineId, false, msgType.client_onAnswerRequestConnect);
            }
        }

        [MessageType(msgType.client_onAnswerRequestConnect)]
        public void onRecive_AnswerRequestConnect(websocketMsgTemp<object> e)
        {
            string remoteMachineId = e.sendMachineId;
            var isAgree = bool.Parse(e.content.ToString());

            if (isAgree == false)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ErrorDialogCon con = new ErrorDialogCon();
                    con.ErrorMsg = "对方不同意链接";
                    DialogHost.Show(con);
                });
                return;
            }
            this.ControllerLogic.ConnectMachine(remoteMachineId);
        }

        [MessageType(msgType.client_onCutPeerConnection)]
        public void onRecive_CutPeerConnection(websocketMsgTemp<object> e)
        {
            string remoteMachineId = e.sendMachineId;
        }


        [MessageType(msgType.client_onCaller_CreateOffer)]
        public void onRecive_CreateOffer(websocketMsgTemp<SdpInfo> e)
        {
            string remoteMachineId = e.sendMachineId;
            //var sdpinfo = JsonConvert.DeserializeObject<SdpInfo>(JsonConvert.SerializeObject(remoteOffer)); 
            var sdpinfo = e.content;
            this.BeControllerLogic.ReciveRemoteConnection(remoteMachineId, sdpinfo);
        }

        [MessageType(msgType.client_onCallee_CreateAnswer)]
        public void onRecive_CreateAnswer(websocketMsgTemp<SdpInfo> e)
        {
            string remoteMachineId = e.sendMachineId;
            //, object remoteAnswer
            var sdpinfo = e.content;
            this.ControllerLogic.SetRemoteAnswer(remoteMachineId, sdpinfo);
        }

        [MessageType(msgType.client_onCaller_CreateIceCandite)]
        public void onRecive_CallerCreateIceCandite(websocketMsgTemp<IceCandidate> e)
        {
            string remoteMachineId = e.sendMachineId;
            var icecandidate = e.content;
            this.BeControllerLogic?.AddRemoteIceCandite(remoteMachineId, icecandidate);
        }

        [MessageType(msgType.client_onCallee_CreateIceCandite)]
        public void onRecive_CalleeCreateIceCandite(websocketMsgTemp<IceCandidate> e)
        {
            string remoteMachineId = e.sendMachineId;
            var icecandidate = e.content;
            this.ControllerLogic?.AddRemoteIceCandite(remoteMachineId, icecandidate);
        }

        [MessageType(msgType.client_onCaller_SetRemoteSdpCompleted)]
        public void onRecive_CallerSetRemoteSdpCompleted(websocketMsgTemp<object> e)
        {
            string sendMachineId = e.sendMachineId;
            this.BeControllerLogic?.onRemoteClientSdpCompleted(sendMachineId);
        }

        [MessageType(msgType.client_onCallee_SetRemoteSdpCompleted)]
        public void onRecive_CalleeSetRemoteSdpCompleted(websocketMsgTemp<object> e)
        {
            string sendMachineId = e.sendMachineId;
            this.ControllerLogic.onRemoteClientSdpCompleted(sendMachineId);
        }

        [MessageType(msgType.system_clientNotOnline)]
        public void onClientNotOnLine(websocketMsgTemp<LYLUserMachineInfo> data)
        {
            ErrorDialogCon con = new ErrorDialogCon();
            con.ErrorMsg = "用户不在线,消息发送失败";
        }

        [MessageType(msgType.system_onGetMyMachine)]
        [MessageType(msgType.system_onMachineOnline)]
        public void onMachineOnLine(websocketMsgTemp<LYLUserMachineInfo> data)
        {
            LYLUserMachineInfo machine = data.content;
            var findmachine = myMachines.FirstOrDefault(o => o.machineId == machine.machineId);
            if (findmachine == null)
            {
                this.myMachines.Add(machine);
            }
            else
            {
                findmachine.machinePwd = machine.machinePwd;
                findmachine.machineName = machine.machineName;
            }
            this.OnPropertyChanged(nameof(this.myMachines));
        }

        [MessageType(msgType.system_onMachineDownline)]
        public void onMachineDownLine(LYLUserMachineInfo machine)
        {
            var findmachine = myMachines.FirstOrDefault(o => o.machineId == machine.machineId);
            if (findmachine != null)
            {
                this.myMachines.Remove(findmachine);
                this.OnPropertyChanged(nameof(this.myMachines));
            }
        }

        [MessageType(msgType.client_onNameChange)]
        public void onMyMachineNameChange(websocketMsgTemp<object> e)
        {
            string sendMachineId = e.sendMachineId;
            object conten = e.content;

            var findmachine = this.myMachines.FirstOrDefault(o => o.machineId == sendMachineId);
            if (findmachine != null)
            {
                findmachine.machineName = conten?.ToString();
                this.myMachines.Remove(findmachine);
                this.myMachines.Insert(0, findmachine);
                this.OnPropertyChanged(nameof(myMachines));
            }
        }
        #endregion

        #region 控件业务处理
        private DelegateCommand _requestConnectOtherCommand => new DelegateCommand(this.startConnectOtherMethod);

        public DelegateCommand requestConnectOtherCommand { get { return this._requestConnectOtherCommand; } }

        private DelegateCommand _requestConnectBrotherCommand => new DelegateCommand(this.startConnectBrotherMethod);

        public DelegateCommand requestConnectBrotherCommand { get { return this._requestConnectBrotherCommand; } }

        private async void startConnectOtherMethod(object obj)
        {
            var machineId = obj?.ToString();
            if (string.IsNullOrEmpty(machineId))
            {
                return;
            }

            var view = new PressStringCon();
            view.InputString = "";
            await DialogHost.Show(view, closingEventHandler: (sender, eventhandle) =>
            {
                this.startConnectMethod(machineId, view.InputString);
            });
        }

        private void startConnectBrotherMethod(object obj)
        {
            var brotherMachine = obj as LYLUserMachineInfo;
            this.startConnectMethod(brotherMachine.machineId, brotherMachine.machinePwd);
        }

        public void startConnectMethod(string machineId, string machinePwd)
        {
            if (string.IsNullOrEmpty(machineId) || string.IsNullOrEmpty(machinePwd))
            {
                return;
            }
            WebSocketClient.SendMessage(machineId, machinePwd, msgType.client_onRequestConnect);
        }

        public void ChangeMachineName(string newName, string oldName)
        {
            if (newName == oldName)
            {
                return;
            }
            ///
            if (string.IsNullOrEmpty(newName))
            {
                ErrorDialogCon errorcon = new ErrorDialogCon();
                errorcon.ErrorMsg = "名称不能为空";
                errorcon.PopupDialog();
                return;
            }
            ///
            if (MachineLogic.ChangeMachineName(MachineLogic.localMachine().machineId, newName) == false)
            {
                ErrorDialogCon errorcon = new ErrorDialogCon();
                errorcon.ErrorMsg = "更新名称错误";
                errorcon.PopupDialog();
                return;
            }
            ///
            foreach (var m in myMachines)
            {
                WebSocketClient.SendMessage(m.machineId, newName, msgType.client_onNameChange);
            }
        }


        #region 直播
        public DelegateCommand srartPlayCommand { get { return new DelegateCommand(this._srartPlayMethod); } }
        private void _srartPlayMethod(object obj)
        {
            NavigationHelper.NavigatedToView("直播主页面");
        }
        #endregion

        #endregion

    }
}
