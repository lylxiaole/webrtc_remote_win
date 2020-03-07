using Controls.Dialogs;
using Dispath;
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
                this.DealMessages(e);
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
                DialogHost.Show(waitingCon, closingEventHandler: (a, b) =>
                {
                    waitingCon = null;
                    if (this.wsclient.state != WebSocketSharp.WebSocketState.Open)
                    {
                        Environment.Exit(0);
                    }
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
        public void DealMessages(LYL.Common.websocketMsgTemp<object> e)
        {
            if (e.msgType == msgType.system_onMachineOnline)
            {
                var data = JsonConvert.DeserializeObject<websocketMsgTemp<LYLUserMachineInfo>>(JsonConvert.SerializeObject(e));
                Application.Current.Dispatcher.Invoke(() => { this.onMachineOnLine(data.content); });
            }
            else if (e.msgType == msgType.system_onMachineDownline)
            {
                var data = JsonConvert.DeserializeObject<websocketMsgTemp<LYLUserMachineInfo>>(JsonConvert.SerializeObject(e));
                Application.Current.Dispatcher.Invoke(() => { this.onMachineDownLine(data.content); });
            }
            else if (e.msgType == msgType.system_onGetMyMachine)
            {
                var data = JsonConvert.DeserializeObject<websocketMsgTemp<LYLUserMachineInfo>>(JsonConvert.SerializeObject(e));
                Application.Current.Dispatcher.Invoke(() => { this.onMachineOnLine(data.content); });
            }
            else if (e.msgType == msgType.system_clientNotOnline)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ErrorDialogCon con = new ErrorDialogCon();
                    con.ErrorMsg = "用户不在线,消息发送失败";
                });
            }
            else if (e.msgType == msgType.client_onRequestConnect)
            {
                this.onRecive_RequestConnect(e.sendMachineId, e.content.ToString());
            }
            else if (e.msgType == msgType.client_onAnswerRequestConnect)
            {
                this.onRecive_AnswerRequestConnect(e.sendMachineId, bool.Parse(e.content.ToString()));
            }
            else if (e.msgType == msgType.client_onCutPeerConnection)
            {
                this.onRecive_CutPeerConnection(e.sendMachineId);
            }
            else if (e.msgType == msgType.client_onCaller_CreateOffer)
            {
                this.onRecive_CreateOffer(e.sendMachineId, e.content);
            }
            else if (e.msgType == msgType.client_onCallee_CreateAnswer)
            {
                this.onRecive_CreateAnswer(e.sendMachineId, e.content);
            }
            else if (e.msgType == msgType.client_onCallee_CreateIceCandite)
            {
                this.onRecive_CalleeCreateIceCandite(e.sendMachineId, e.content);
            }
            else if (e.msgType == msgType.client_onCaller_CreateIceCandite)
            {
                this.onRecive_CallerCreateIceCandite(e.sendMachineId, e.content);
            }
            else if (e.msgType == msgType.client_onCallee_SetRemoteSdpCompleted)
            {
                this.onRecive_CalleeSetRemoteSdpCompleted(e.sendMachineId, e.content);
            }
            else if (e.msgType == msgType.client_onCaller_SetRemoteSdpCompleted)
            {
                this.onRecive_CallerSetRemoteSdpCompleted(e.sendMachineId, e.content);
            }
            else if (e.msgType == msgType.client_onNameChange)
            {
                this.onMyMachineNameChange(e.sendMachineId, e.content);
            }
        }

        private void onRecive_RequestConnect(string remoteMachineId, string machinePwd)
        {
            if (machinePwd == this.CurrentMachine.machinepwd && this.BeControllerLogic.CanConnect == true)
            {
                WebSocketClient.SendMessage(remoteMachineId, true, msgType.client_onAnswerRequestConnect);
            }
            else
            {
                WebSocketClient.SendMessage(remoteMachineId, false, msgType.client_onAnswerRequestConnect);
            }
        }
        private void onRecive_AnswerRequestConnect(string remoteMachineId, bool isAgree)
        {
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
        private void onRecive_CutPeerConnection(string remoteMachineId)
        {
        }
        private void onRecive_CreateOffer(string remoteMachineId, object remoteOffer)
        {
            var sdpinfo = JsonConvert.DeserializeObject<SdpInfo>(JsonConvert.SerializeObject(remoteOffer));
            this.BeControllerLogic.ReciveRemoteConnection(remoteMachineId, sdpinfo);
        }
        private void onRecive_CreateAnswer(string remoteMachineId, object remoteAnswer)
        {
            var sdpinfo = JsonConvert.DeserializeObject<SdpInfo>(JsonConvert.SerializeObject(remoteAnswer));
            this.ControllerLogic.SetRemoteAnswer(remoteMachineId, sdpinfo);
        }
        private void onRecive_CallerCreateIceCandite(string remoteMachineId, object remoteIceCandite)
        {
            var icecandidate = JsonConvert.DeserializeObject<IceCandidate>(JsonConvert.SerializeObject(remoteIceCandite));
            this.BeControllerLogic?.AddRemoteIceCandite(remoteMachineId, icecandidate);
        }

        private void onRecive_CalleeCreateIceCandite(string remoteMachineId, object remoteIceCandite)
        {
            var icecandidate = JsonConvert.DeserializeObject<IceCandidate>(JsonConvert.SerializeObject(remoteIceCandite));
            this.ControllerLogic?.AddRemoteIceCandite(remoteMachineId, icecandidate);
        }

        private void onRecive_CallerSetRemoteSdpCompleted(string sendMachineId, object content)
        {
            this.BeControllerLogic?.onRemoteClientSdpCompleted(sendMachineId);
        }

        private void onRecive_CalleeSetRemoteSdpCompleted(string sendMachineId, object content)
        {
            this.ControllerLogic.onRemoteClientSdpCompleted(sendMachineId);
        }

        private void onMachineOnLine(LYLUserMachineInfo machine)
        {
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
        private void onMachineDownLine(LYLUserMachineInfo machine)
        {
            var findmachine = myMachines.FirstOrDefault(o => o.machineId == machine.machineId);
            if (findmachine != null)
            {
                this.myMachines.Remove(findmachine);
                this.OnPropertyChanged(nameof(this.myMachines));
            }
        }

        private void onMyMachineNameChange(string sendMachineId, object conten)
        {
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
        #endregion

    }
}
