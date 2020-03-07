using System;
using System.IO;
using System.Runtime.InteropServices;
using WebrtcSDK_NET.WebRtc;

namespace WebrtcSDK_NET
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AudioDevice
    {
        public AudioDevice(bool isctor)
        {
            name = "";
            guid = "";
        }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string guid;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct VedioDevice
    {
        public VedioDevice(bool isctor)
        {
            deviceNameUTF8 = "";
            deviceNameLength = 256;
            deviceUniqueIdUTF8 = "";
            deviceUniqueIdUTF8Length = 256;
            productUniqueIdUTF8 = "";
            productUniqueIdUTF8Length = 256;
        }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string deviceNameUTF8;

        public UInt32 deviceNameLength;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string deviceUniqueIdUTF8;

        public UInt32 deviceUniqueIdUTF8Length;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string productUniqueIdUTF8;

        public UInt32 productUniqueIdUTF8Length;
    };

 

    /// <summary>
    /// 
    /// </summary>
    /// 
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sdpCreateSuccess_Callback(string sdp, string type);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sdpCreateFailure_Callback(string erro);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void setSdpSuccess_Callback();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void setSdpFailure_Callback(string error);

    /// <summary>
    /// 管道委托
    /// </summary>
    ///    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onChannelStateChanged_Callback(RTCDataChannelState state, string channelLabel);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onChannelMessage_Callback(string buffer, Int32 length, bool binary, string channelLabel);

    /// <summary>
    /// rtcconnection委托
    /// </summary>
    /// <param name="state"></param>
    /// <param name="connectionAddr"></param>
    /// 
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onSignalingState_Callback(Int32 state);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onIceGatheringState_Callback(Int32 state);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onRemoveStream_Callback(string streamLabel);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onRemoveTrack_Callback(string streamLabel, string trackId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onDataChannel_Callback(string data_channelId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onAddTrack_Callback(string streamLabel, string trackId, string trackType);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onAddStream_Callback(string streamLabel);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onIceConnectionState_Callback(int state);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onIceCandidate_Callback(string candidate, string sdp_mid, int sdp_mline_index);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void onFrame_Callback(IntPtr rgbImg, Int32 buffer_size, Int32 width, Int32 height, string trackId);

    public class libwebrtcNET
    {
        #region 全局静态方法
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Initialize();

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RealseAll();
        #endregion
         
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Int32 CreatePeerConnection();

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterIceServer([MarshalAs(UnmanagedType.LPStr)]string uri, [MarshalAs(UnmanagedType.LPStr)]string username, [MarshalAs(UnmanagedType.LPStr)]string password, Int32 peeraddr);
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPeerConnectionConfig(int icetype, Int32 peeraddr);
         
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetTrackDevice(bool isVedio, int vedioIndex, bool isAudio, int recordAudioIndex, bool isDesktop, Int32 peeraddr);

        #region 视频语音设备相关
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayoutAudioDevice(Int32 playoutAudioIndex);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetVideoDeviceNumber();

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetVideoDevice(Int32 index, ref VedioDevice refdeviceInfo);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPlayoutAudioDeviceNumber();

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetPlayoutAudioDevice(Int32 index, ref AudioDevice refdeviceInfo);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetRecordingAudioDeviceNumber();

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetRecordingAudioDevice(Int32 index, ref AudioDevice refdeviceInfo);
        #endregion

        #region 信令相关
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void CreateOffer([MarshalAs(UnmanagedType.FunctionPtr)]sdpCreateSuccess_Callback successcallback, [MarshalAs(UnmanagedType.FunctionPtr)]sdpCreateFailure_Callback failurecallback, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void AddCandidate([MarshalAs(UnmanagedType.LPStr)]string candiate, [MarshalAs(UnmanagedType.LPStr)]string mid, int midx, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetRemoteDescription([MarshalAs(UnmanagedType.LPStr)]string offer_sdp, [MarshalAs(UnmanagedType.LPStr)]string type,
                                                       [MarshalAs(UnmanagedType.FunctionPtr)]sdpCreateSuccess_Callback onCreateSdpSuccess,
                                                       [MarshalAs(UnmanagedType.FunctionPtr)]sdpCreateFailure_Callback onCreateSdpFailure, Int32 peeraddr);
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DealCacheIceCandidate(Int32 peeraddr);
        #endregion

        #region 管道处理 
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateDataChannel([MarshalAs(UnmanagedType.LPStr)]string channelLabel, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SendChannelData([MarshalAs(UnmanagedType.LPStr)]string channelLabel, [MarshalAs(UnmanagedType.LPStr)]string data, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetDataChannelEvent(onChannelStateChanged_Callback onStateChangeHandle, onChannelMessage_Callback onMessageHandle, [MarshalAs(UnmanagedType.LPStr)]string channelLabel, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RemoveDataChannel([MarshalAs(UnmanagedType.LPStr)]string channelLabel, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RemoveStream([MarshalAs(UnmanagedType.LPStr)]string streamId, Int32 peeraddr);
        #endregion

        #region 注册connection事件
        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerSignalingState(onSignalingState_Callback handle, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerIceGatheringState(onIceGatheringState_Callback handle, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerRemoveStream(onRemoveStream_Callback handle, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerRemoveTrack(onRemoveTrack_Callback handle, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerOnNewDataChannel(onDataChannel_Callback handle, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerAddTrack(onAddTrack_Callback handle, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerAddStream(onAddStream_Callback handle, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerIceConnectionState(onIceConnectionState_Callback handle, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenerIceCandidate(onIceCandidate_Callback onIceCandidate, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenonLocalFrame(onFrame_Callback onFrameMethod, Int32 peeraddr);

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetListenonRemoteFrame(onFrame_Callback onFrameMethod, [MarshalAs(UnmanagedType.LPStr)]string trackId, Int32 peeraddr);
        #endregion

        [DllImport("lylWebrtcClientDll.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void CloseConnection(Int32 peeraddr);

    }
}
