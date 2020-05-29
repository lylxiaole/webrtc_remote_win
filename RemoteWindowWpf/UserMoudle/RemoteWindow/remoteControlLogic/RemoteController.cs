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
using Dispath;
using LYL.Common;
using LYL.Logic.Machine;
using MaterialDesignThemes.Wpf;
using MouseKeyPlayback;
using Newtonsoft.Json;
using UserMoudle.RemoteWindow.remoteControlLogic;
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.RemoteWindow
{
    /// <summary>
    /// 控制着
    /// </summary>
    public class RemoteController : RemoteControlBase
    { 
        private Image _remoteImageControl;
        public Image RemoteImageControl
        {
            get
            { 
                return _remoteImageControl; 
            }
            set
            {   
                _remoteImageControl = value; 
            }
        }

        public MediaStream currentMediaStream { get; set; }

        public RemoteController() : base()
        {
            //this.IsDesktop = true;
            //this.IsAudio = true;
        }
        #region 处理peerconnection
        /// <summary>
        /// 主动呼叫
        /// </summary>
        /// <param name="mmachineId"></param>
        public bool ConnectRemote(string machineId)
        {
            this.remoteMachine = MachineLogic.GetMachineById(machineId);
            if (this.remoteMachine == null)
            {
                return false;
            }
            this.InitlizeConnetion();
            this.keyMouseChannel = this.PeerConnection.CreateDataChannel(ChannelName.keyMouseChannel.ToString());
            this.fileChannel = this.PeerConnection.CreateDataChannel(ChannelName.fileChannel.ToString());
            FileChannelMessageDeal.RegisterFileChannel(fileChannel);
            var offerInfo = this.PeerConnection.CreateOffer().Result;
            if (offerInfo == null)
            {
                this.showInfo("流媒体设备启动失败");
            }
            WebSocketClient.SendMessage(machineId, offerInfo, msgType.client_onCaller_CreateOffer);
            return true;
        }

        /// <summary>
        /// 主动连接对方成功，接受对方发来的answer，完成sdp交换
        /// </summary>
        /// <param name="mchineId"></param>
        /// <param name="offerinfo"></param>
        public bool SetRemoteAnswer(string machineId, string sdp, string type)
        {
            this.PeerConnection.SetRemoteDescription(sdp, type).Wait();
            WebSocketClient.SendMessage(machineId, "", msgType.client_onCaller_SetRemoteSdpCompleted);
            return true;
        }

      

        protected override void onAddStreamEvent(object sender, MediaStream e)
        {
            base.onAddStreamEvent(sender, e);
            if (this.currentMediaStream != null)
            {
                throw new Exception("一个链接不允许有多个媒体流");
            }
            this.currentMediaStream = e;
            currentMediaStream.onRemoteVideoFrame += E_onRemoteFrame;
        }

        protected override void onRemoveStreamEvent(object sender, string e)
        {
            base.onRemoveStreamEvent(sender, e);
            if (this.currentMediaStream == null)
            {
                return;
            }
            currentMediaStream.onRemoteVideoFrame -= E_onRemoteFrame;
            this.currentMediaStream.Close();
        }

        protected override void onIceCandidateEvent(object sender, IceCandidate e)
        {
            if (this.remoteMachine == null)
            {
                return;
            }
            WebSocketClient.SendMessage(this.remoteMachine.machineId, e, msgType.client_onCaller_CreateIceCandite);
        }

        protected override void PeerConnection_onNewDataChannel(object sender, DataChannel e)
        {
            base.PeerConnection_onNewDataChannel(sender, e);
            //e.onChannelMessage += E_onChannelMessage;
            //e.onChannelStateChanged += E_onChannelStateChanged;
        }

        private void E_onChannelStateChanged(object sender, RTCDataChannelState e)
        {
        }
        #endregion


        private void E_onRemoteFrame(object sender, VedioFrame e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.RemoteImageControl != null)
                {
                    this.RemoteImageControl.Source = VideoFrameDeal.dealImageByte(e.rgbImg, e.width, e.height);
                }
            });
        }

        #region 发送鼠标键盘事件
        public void SendWinApiEvents(List<Record> events)
        {
            this.keyMouseChannel.SendChannelData(JsonConvert.SerializeObject(events));
        }
        #endregion


    }
}
