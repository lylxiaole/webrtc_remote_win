using System;
using System.Collections.Generic;
using System.Text;

namespace LYL.Common
{
    /// <summary>
    /// webSocket消息类型:
    /// 0-999为系统消息,
    /// 1000-9999为客户端消息
    /// </summary> 
    public enum msgType
    {
        /// <summary>
        /// 当自己的设备上线
        /// </summary>
        system_onMachineOnline = 0,
        /// <summary>
        /// 当自己的设备下线
        /// </summary>
        system_onMachineDownline = 1,
        /// <summary>
        /// 当获取到自己在线的设备
        /// </summary>
        system_onGetMyMachine = 2,
        /// <summary>
        /// 客户端不在线
        /// </summary>
        system_clientNotOnline = 3,

        /// <summary>
        /// 开始直播
        /// </summary>
        system_startPlay = 50,
        /// <summary>
        /// 直播服务端创建offer成功
        /// </summary>
        system_onPlayServerOfferCreated = 51,
        /// <summary>
        /// 直播端创建answer给服务端
        /// </summary>
        system_setPlayServerAnswer = 52,
        /// <summary>
        /// 直播服务端创建了icecandidate
        /// </summary>
        system_onPlayServerIcecandidate = 53,
        /// <summary>
        /// 直播端创建了icecandidate
        /// </summary>
        system_onPlayerIcecandidate = 54, 
        /// <summary>
        /// 观看直播
        /// </summary> 
        system_onWatchPlay = 60,
        /// <summary>
        /// 观看者offer创建成功
        /// </summary>
        system_onWatcherOfferCreated = 61,
        /// </summary>
        system_onWatcherIcecandidate = 62,
        /// <summary>
        /// 直播端创建answer给服务端
        /// </summary>
        system_setWatcherAnswer = 63,
        //
        client_onRequestConnect = 1000,
        client_onAnswerRequestConnect = 1001,
        client_onCutPeerConnection = 1002,
        //
        client_onCaller_CreateOffer = 1003,
        client_onCallee_CreateAnswer = 1004, 
        client_onCaller_CreateIceCandite = 1005,
        client_onCallee_CreateIceCandite = 1006, 
        client_onCaller_SetRemoteSdpCompleted = 1007,
        client_onCallee_SetRemoteSdpCompleted = 1008,
        //
        client_onNameChange=1050

    }

    public class websocketMsgTemp<T>
    {
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public msgType msgType { get; set; }
        public T content { get; set; }
    }
}
