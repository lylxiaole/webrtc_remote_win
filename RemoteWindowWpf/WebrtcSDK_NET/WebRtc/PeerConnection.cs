using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WebrtcSDK_NET.WebRtc
{
    public class PeerConnection
    {
        static PeerConnection()
        {
            libwebrtcNET.Initialize();
        }

        ~PeerConnection()
        {
            this.Close();
        }
        static WebRtcConfig _config { get; set; }
        public static void RegisterIceServer(WebRtcConfig config)
        {
            _config = config;
        }

        private IntPtr _connectionAddr { get; set; }

    
        public List<MediaStream> RemoteMediaStreams { get; set; } = new List<MediaStream>();

        public RTCIceConnectionState ConnectionState { get; set; } = RTCIceConnectionState.RTCIceConnectionStateClosed;

        public PeerConnection(bool isVedio, int vedioIndex, bool isAudio, Int32 recordAudioIndex, bool isDesktop)
        {
            this._connectionAddr = libwebrtcNET.CreatePeerConnection();
            libwebrtcNET.SetTrackDevice(isVedio, vedioIndex, isAudio, recordAudioIndex, isDesktop, this._connectionAddr);
            libwebrtcNET.SetPeerConnectionConfig((int)_config.iceTransportPolicy, this._connectionAddr);
            foreach (var vv in _config.iceServers)
            {
                libwebrtcNET.RegisterIceServer(vv.url, vv.username, vv.credential, this._connectionAddr);
            }
            /////
            libwebrtcNET.SetLocalVideoTrackRGBAHandle(this.onLocalVideoRgba_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetLocalDesktopTrackRGBAHandle(this.onLocalDesktopRgba_Callback_Handle, this._connectionAddr);
            /////
            libwebrtcNET.SetListenerAddStream(this.onAddStream_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetListenerOnNewDataChannel(this.onDataChannel_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetListenerIceCandidate(this.onIceCandidate_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetListenerIceConnectionState(this.onIceConnectionState_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetListenerIceGatheringState(this.onIceGatheringState_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetListenerRemoveStream(this.onRemoveStream_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetListenerRemoveTrack(this.onRemoveTrack_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetListenerSignalingState(this.onSignalingState_Callback_Handle, this._connectionAddr);
            libwebrtcNET.SetListenerAddTrack(this.onAddTrack_Callback_Hanle, this._connectionAddr);
            libwebrtcNET.InitializePeerConnection(this._connectionAddr); 
        }
        //******************************设备相关***************************
        public List<VedioDevice> GetAllVedioDevices()
        {
            List<VedioDevice> vedios = new List<VedioDevice>();
            var vedioCount = libwebrtcNET.GetVideoDeviceNumber();
            for (int i = 0; i < vedioCount; i++)
            {
                VedioDevice vedio = new VedioDevice(true);
                libwebrtcNET.GetVideoDevice(i, ref vedio);
                vedios.Add(vedio);
            }
            return vedios;
        }
        public List<AudioDevice> GetAllRecordingAudioDevices()
        {
            List<AudioDevice> recordingDevices = new List<AudioDevice>();
            var audioCount = libwebrtcNET.GetRecordingAudioDeviceNumber();
            for (int i = 0; i < audioCount; i++)
            {
                AudioDevice audio = new AudioDevice(true);
                libwebrtcNET.GetRecordingAudioDevice(i, ref audio);
                recordingDevices.Add(audio);
            }
            return recordingDevices;
        }
        public List<AudioDevice> GetAllPlayoutAudioDevices()
        {
            List<AudioDevice> playoutDevices = new List<AudioDevice>();
            var audioCount = libwebrtcNET.GetPlayoutAudioDeviceNumber();
            for (int i = 0; i < audioCount; i++)
            {
                AudioDevice audio = new AudioDevice(true);
                libwebrtcNET.GetPlayoutAudioDevice(i, ref audio);
                playoutDevices.Add(audio);
            }
            return playoutDevices;
        }
        //******************************peerconnection信令事件***************************
        #region 对外开放的事件  
        public event EventHandler<MediaStream> onAddStream;
        public event EventHandler<string> onRemoveStream;
        public event EventHandler<Track> onAddTrack;
        public event EventHandler<Track> onRemoveTrack;
        public event EventHandler<DataChannel> onNewDataChannel;
        public event EventHandler<IceCandidate> onIceCandidate;
        public event EventHandler<RTCIceConnectionState> onIceConnectionState;
        public event EventHandler<RTCIceGatheringState> onIceGatheringState;
        public event EventHandler<RTCSignalingState> onSignalingState;
        public event EventHandler<VedioFrame> onLocalVideoRgbaFrame;
        public event EventHandler<VedioFrame> onLocalDesktopRgbaFrame;
        #endregion
        #region eventHandle,用于调用libwebrtc注册事件 
        private List<DataChannel> _channels = new List<DataChannel>();
        /// <summary>
        /// 只有这样写，委托才不会被GC换了地址
        /// </summary>
        private onAddStream_Callback _onAddStream_Callback_Handle;
        private onAddStream_Callback onAddStream_Callback_Handle
        {
            get => _onAddStream_Callback_Handle = (string streamId) =>
              {
                  //var mediaStream = this.RemoteMediaStreams.FirstOrDefault(o => o.streamId == streamId);
                  //if (mediaStream == null)
                  //{
                  //    mediaStream = new MediaStream(streamId, this._connectionAddr);
                  //    this.RemoteMediaStreams.Add(mediaStream); 
                  //} 
                  //this.onAddStream?.Invoke(this, mediaStream);
              };
        }

        private onAddTrack_Callback _onAddTrack_Callback_Hanle;
        private onAddTrack_Callback onAddTrack_Callback_Hanle
        {
            get => _onAddTrack_Callback_Hanle = (string streamId, string trackId, string trackType) =>
              {
                  var mediaStream = this.RemoteMediaStreams.FirstOrDefault(o => o.streamId == streamId);

                  if (mediaStream == null)
                  {
                      mediaStream = new MediaStream(streamId, this._connectionAddr);
                      this.RemoteMediaStreams.Add(mediaStream);
                      this.onAddStream?.Invoke(this, mediaStream);
                  }

                  var track = new Track
                  {
                      mediaStream = mediaStream,
                      trackId = trackId,
                      trackType = trackType
                  };

                  mediaStream.AddTrack(track);
                  this.onAddTrack?.Invoke(this, track);
              };
        }

        private onDataChannel_Callback _onDataChannel_Callback_Handle;
        private onDataChannel_Callback onDataChannel_Callback_Handle
        {
            get => _onDataChannel_Callback_Handle = (string data_channelId) =>
              {
                  var channel = new DataChannel(data_channelId, this._connectionAddr);
                  channel.StartListenEvent();
                  this._channels.Add(channel);
                  this.onNewDataChannel?.Invoke(this, channel);
              };
        }

        private onIceCandidate_Callback _onIceCandidate_Callback_Handle;
        private onIceCandidate_Callback onIceCandidate_Callback_Handle
        {
            get => _onIceCandidate_Callback_Handle = (string candidate, string sdp_mid, int sdp_mline_index) =>
              {
                  var candidation = new IceCandidate { candidate = candidate, sdpMid = sdp_mid, sdpMLineIndex = sdp_mline_index };
                  this.onIceCandidate?.Invoke(this, candidation);
              };
        }

        private onIceConnectionState_Callback _onIceConnectionState_Callback_Handle;
        private onIceConnectionState_Callback onIceConnectionState_Callback_Handle
        {
            get => _onIceConnectionState_Callback_Handle = (int state) =>
              {
                  var stateEnum = (RTCIceConnectionState)state;
                  this.ConnectionState = stateEnum;
                  //if (stateEnum == RTCIceConnectionState.RTCIceConnectionStateCompleted)
                  //{
                  //    //
                  //    this._channels.ForEach(o => o.StartListenEvent());
                  //}
                  if (stateEnum == RTCIceConnectionState.RTCIceConnectionStateDisconnected || stateEnum == RTCIceConnectionState.RTCIceConnectionStateClosed || stateEnum == RTCIceConnectionState.RTCIceConnectionStateFailed)
                  {
                      //
                      this.Close();
                  }
                  this.onIceConnectionState?.Invoke(this, stateEnum);
              };
        }

        private onIceGatheringState_Callback _onIceGatheringState_Callback_Handle;
        private onIceGatheringState_Callback onIceGatheringState_Callback_Handle
        {
            get => _onIceGatheringState_Callback_Handle = (Int32 state) =>
             {
                 this.onIceGatheringState?.Invoke(this, (RTCIceGatheringState)state);
             };
        }

        private onRemoveStream_Callback _onRemoveStream_Callback_Handle;
        private onRemoveStream_Callback onRemoveStream_Callback_Handle
        {
            get => _onRemoveStream_Callback_Handle = (string streamId) =>
              {
                  this.onRemoveStream?.Invoke(this, streamId);
              };
        }

        private onRemoveTrack_Callback _onRemoveTrack_Callback_Handle;
        private onRemoveTrack_Callback onRemoveTrack_Callback_Handle
        {
            get => _onRemoveTrack_Callback_Handle = (string streamId, string trackId) =>
              {
                  var mediaStream = this.RemoteMediaStreams.FirstOrDefault(o => o.streamId == streamId);
                  var track = new Track { mediaStream = mediaStream, trackId = trackId };
                  this.onRemoveTrack?.Invoke(this, track);
              };
        }

        private onSignalingState_Callback _onSignalingState_Callback_Handle;
        private onSignalingState_Callback onSignalingState_Callback_Handle
        {
            get => _onSignalingState_Callback_Handle = (Int32 state) =>
               {
                   this.onSignalingState?.Invoke(this, (RTCSignalingState)state);
               };
        }

        private onFrame_Callback _onLocalVideoRgba_Callback_Handle;
        private onFrame_Callback onLocalVideoRgba_Callback_Handle
        {
            get => _onLocalVideoRgba_Callback_Handle = (IntPtr rgbImg, Int32 buffer_size, Int32 width, Int32 height, string vedioTrackId) =>
              {
                  if (this.onLocalVideoRgbaFrame != null)
                  {
                      byte[] data = new byte[buffer_size];
                      Marshal.Copy(rgbImg, data, 0, buffer_size);
                      var lframe = new VedioFrame { rgbImg = data, width = width, height = height, vedioTrackId = vedioTrackId };
                      this.onLocalVideoRgbaFrame?.Invoke(this, lframe);
                  }
              };
        }

        private onFrame_Callback _onLocalDesktopRgba_Callback_Handle;
        private onFrame_Callback onLocalDesktopRgba_Callback_Handle
        {
            get => _onLocalDesktopRgba_Callback_Handle = (IntPtr rgbImg, Int32 buffer_size, Int32 width, Int32 height, string vedioTrackId) =>
              {
                  if (this.onLocalVideoRgbaFrame != null)
                  {
                      byte[] data = new byte[buffer_size];
                      Marshal.Copy(rgbImg, data, 0, buffer_size);
                      var lframe = new VedioFrame { rgbImg = data, width = width, height = height, vedioTrackId = vedioTrackId };
                      this.onLocalDesktopRgbaFrame?.Invoke(this, lframe);
                  }
              };
        }
        #endregion
        //******************************peerconnection对外连接方法***************************
        public Task<SdpInfo> CreateOffer()
        {
            return Task.Run<SdpInfo>(() =>
            {
                AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
                SdpInfo sdpinfo = null;
                libwebrtcNET.CreateOffer((string sdp, string type) =>
                 {
                     sdpinfo = new SdpInfo
                     {
                         sdp = sdp,
                         type = type
                     };
                     _autoResetEvent.Set();
                 },
                 (string erro) =>
                 {
                     _autoResetEvent.Set();
                 }, this._connectionAddr);
                _autoResetEvent.WaitOne();
                return sdpinfo;
            });
        }
        public void AddCandidate(IceCandidate iceCandidate)
        {
            libwebrtcNET.AddCandidate(iceCandidate.candidate, iceCandidate.sdpMid, iceCandidate.sdpMLineIndex, this._connectionAddr);
        }
        public Task<SdpInfo> SetRemoteDescription(string sdp, string type)
        {
            return Task.Run<SdpInfo>(() =>
            {
                SdpInfo answersdp = null;
                if (type == "offer")
                {
                    AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
                    libwebrtcNET.SetRemoteDescription(sdp, type, (string answersdpstr, string answertypestr) =>
                    {
                        answersdp = new SdpInfo
                        {
                            sdp = answersdpstr,
                            type = answertypestr
                        };
                        _autoResetEvent.Set();
                    },
                     (string erro) =>
                     {
                         _autoResetEvent.Set();
                     }, this._connectionAddr);

                    _autoResetEvent.WaitOne();
                }
                else
                {
                    libwebrtcNET.SetRemoteDescription(sdp, type, null, null, this._connectionAddr);
                }
                return answersdp;
            });

        }
        public DataChannel CreateDataChannel(string channel_label)
        {
            var channelIdptr = libwebrtcNET.CreateDataChannel(channel_label, this._connectionAddr);
            var channelId = Marshal.PtrToStringAnsi(channelIdptr);
            var newChannel = new DataChannel(channelId, this._connectionAddr);
            newChannel.label = channel_label;
            newChannel.StartListenEvent();
            _channels.Add(newChannel);
            return newChannel;
        }

        public void RemoveDataChannel(DataChannel channel)
        {
            libwebrtcNET.RemoveDataChannel(channel.label, this._connectionAddr);
            this._channels.Remove(channel);
        }

        public int AddStream(MediaStream stream)
        {
            var ptr = libwebrtcNET.AddStream(stream.connectionAddr, stream.streamId, this._connectionAddr);

            return ptr;
        }

        private bool isClosed = false;
        public void Close()
        {
            Task.Run(() =>
            {
                if (isClosed == true)
                {
                    return;
                }
                isClosed = true;
                libwebrtcNET.CloseConnection(this._connectionAddr);
                isClosed = true;
            });
        }
    }
}
