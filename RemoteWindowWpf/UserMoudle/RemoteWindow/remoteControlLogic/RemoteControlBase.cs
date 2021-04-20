
using Controls.Dialogs;
using Dispath;
using LYL.Common;
using LYL.Data.Models;
using LYL.Logic;
using LYL.Logic.Machine;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UserMoudle.RemoteWindow.remoteControlLogic;
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.RemoteWindow
{
    public enum ChannelName
    {
        keyMouseChannel = 1,
        fileChannel = 2
    }
    public class RemoteControlBase : ViewModelBase
    {
        //*******************************************************************************
        protected PeerConnection PeerConnection { get; set; }
        protected WebRtcConfig config { get; set; }

        public DataChannel keyMouseChannel { get; set; }
        public DataChannel fileChannel { get; set; }

        public Guid Id { get; set; }
        protected RemoteControlBase()
        {
            this.Id = Guid.NewGuid();
            this.config = new WebRtcConfig();
            this.config.iceServers = new List<iceServer>();
            //this.config.iceServers.Add(new iceServer
            //{
            //    url = "turn:" + ServerAddrs.lylRtcServerAddr + "?transport=udp",
            //    username = MachineLogic.localMachine().machineId,
            //    credential = MachineLogic.localMachine().machineId,

            //});



            this.config.iceServers.Add(new iceServer
            {
                url = "stun:" + ServerAddrs.lylRtcServerAddr + "?transport=udp",
                username = "lyl.org",
                credential = "lyl.org",

            });
            this.config.iceTransportPolicy = iceTransportPolicyType.kAll;
            PeerConnection.RegisterIceServer(this.config);
        }
        protected void InitlizeConnetion()
        {
            if (this.PeerConnection == null)
            {
                /************/
                this.PeerConnection = new PeerConnection(IsVideo, 0, IsAudio, 0, IsDesktop);
                try
                {
                    this.PeerConnection.onNewDataChannel += onDataChannelEvent;
                    this.PeerConnection.onIceCandidate += onIceCandidateEvent;
                    this.PeerConnection.onIceConnectionState += onIceConnectionStateEvent;
                    this.PeerConnection.onIceGatheringState += onIceGatheringStateEvent;
                    this.PeerConnection.onLocalDesktopRgbaFrame += onLocalFrameEvent;
                    this.PeerConnection.onAddStream += onAddStreamEvent;
                    this.PeerConnection.onAddTrack += onAddTrackEvent;
                    this.PeerConnection.onRemoveStream += onRemoveStreamEvent;
                    this.PeerConnection.onRemoveTrack += onRemoveTrackEvent;
                    this.PeerConnection.onSignalingState += onSignalingStateEvent;
                    this.PeerConnection.onNewDataChannel += PeerConnection_onNewDataChannel;
                }
                catch (Exception)
                {
                    throw new Exception("创建失败");
                }
            }
        }
        #region peerconnetion事件处理
        protected virtual void onSignalingStateEvent(object sender, RTCSignalingState e)
        {
        }
        protected virtual void onIceGatheringStateEvent(object sender, RTCIceGatheringState e)
        {

        }
        protected virtual void onIceConnectionStateEvent(object sender, RTCIceConnectionState e)
        {
            if (e == RTCIceConnectionState.RTCIceConnectionStateCompleted)
            {

            }
            else if (e == RTCIceConnectionState.RTCIceConnectionStateDisconnected || e == RTCIceConnectionState.RTCIceConnectionStateClosed || e == RTCIceConnectionState.RTCIceConnectionStateFailed)
            {
                this.onCloseEvent?.Invoke(this, this.Id);
            }
        }
        protected virtual void onIceCandidateEvent(object sender, IceCandidate e)
        {
        }
        protected virtual void onDataChannelEvent(object sender, DataChannel e)
        {
        }
        protected virtual void onLocalFrameEvent(object sender, VedioFrame e)
        {
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //var bitmap = VideoFrameDeal.dealImageByte(e.rgbImg, e.width, e.height);
            //this.localBitmap = bitmap;
            //this.OnPropertyChanged(nameof(this.localBitmap));

            //});
        }
        protected virtual void onAddTrackEvent(object sender, Track e)
        {
            if (e.trackType == "video")
            {

            }
        }
        protected virtual void onRemoveTrackEvent(object sender, Track e)
        {
        }
        protected virtual void onAddStreamEvent(object sender, MediaStream e)
        {

        }
        protected virtual void PeerConnection_onNewDataChannel(object sender, DataChannel e)
        {
            switch (e.label)
            {
                case nameof(ChannelName.fileChannel):
                    {
                        this.fileChannel = e;
                    }
                    break;
                case nameof(ChannelName.keyMouseChannel):
                    {
                        this.keyMouseChannel = e;
                    }
                    break;
                default:
                    break;
            }
        }
        protected virtual void onRemoveStreamEvent(object sender, string e)
        {
        }
        #endregion
        protected void showInfo(string info)
        {
            ErrorDialogCon con = new ErrorDialogCon();
            con.ErrorMsg = info;
            con.PopupDialog();
        }
        //************************************************************************************************
        public bool IsAudio { get; set; }
        public bool IsVideo { get; set; }
        public bool IsDesktop { get; set; }
        public MachineInfo remoteMachine { get; set; }
        public WriteableBitmap localBitmap { get; set; }
        public RTCIceConnectionState State
        {
            get
            {
                if (this.PeerConnection == null)
                {
                    return RTCIceConnectionState.RTCIceConnectionStateClosed;
                }
                return this.PeerConnection.ConnectionState;
            }
        }
        /// <summary>
        /// 接收到对方的Candidate，进行处理
        /// </summary>
        /// <param name="candidate"></param>
        public void AddRemoteCandidate(string mchineId, IceCandidate iceCandidate)
        {
            var ice = JsonConvert.SerializeObject(iceCandidate);
            if (mchineId == MachineLogic.localMachine().machineId)
            {
                return;
            }
            this.PeerConnection.AddCandidate(iceCandidate);

        }
    
        public void CloseConnection()
        {
            this.PeerConnection.Close();
            FileChannelMessageDeal.removeFileChannel(this.fileChannel);
        }
        public event EventHandler<Guid> onCloseEvent;

    }
}