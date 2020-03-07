using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebrtcSDK_NET.WebRtc
{
    public class SdpInfo
    {
        public string sdp { get; set; }
        public string type { get; set; }
    }

    public class IceCandidate
    {
        public string candidate { get; set; }
        public string sdpMid { get; set; }

        public int sdpMLineIndex { get; set; }
    }

    public class Track
    {
        public MediaStream mediaStream { get; set; }
        public string trackId { get; set; }
        public string trackType { get; set; }
        
    }

    public class VedioFrame
    {
        public byte[] rgbImg { get; set; }
        public string vedioTrackId { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public enum RTCSignalingState
    {
        RTCSignalingStateStable = 0,
        RTCSignalingStateHaveLocalOffer = 1,
        RTCSignalingStateHaveRemoteOffer = 2,
        RTCSignalingStateHaveLocalPrAnswer = 3,
        RTCSignalingStateHaveRemotePrAnswer = 4,
        RTCSignalingStateClosed = 5
    };

    public enum RTCIceGatheringState
    {
        RTCIceGatheringStateNew = 0,
        RTCIceGatheringStateGathering = 1,
        RTCIceGatheringStateComplete = 2
    };

    public enum RTCIceConnectionState
    {
        RTCIceConnectionStateNew = 0,
        RTCIceConnectionStateChecking = 1,
        RTCIceConnectionStateCompleted = 2,
        RTCIceConnectionStateConnected = 3,
        RTCIceConnectionStateFailed = 4,
        RTCIceConnectionStateDisconnected = 5,
        RTCIceConnectionStateClosed = 6,
        RTCIceConnectionStateMax = 7,
    };

    public enum RTCDataChannelState
    {
        RTCDataChannelConnecting=0,
        RTCDataChannelOpen=1,
        RTCDataChannelClosing=2,
        RTCDataChannelClosed=3,
    };

    public class ChannelMessage
    {
        public string buffer { get; set; }
        public Int32 length { get; set; }
        public bool binary { get; set; }
    }

}
