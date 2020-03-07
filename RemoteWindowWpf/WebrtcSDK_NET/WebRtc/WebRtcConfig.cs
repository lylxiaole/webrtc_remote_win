using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebrtcSDK_NET.WebRtc
{
    public enum iceTransportPolicyType
    {
        kNone = 0,
        kRelay = 1,
        kNoHost2,
        kAll = 3
    }

    public class iceServer
    {
        public string url { get; set; }
        public string username { get; set; }
        public string credential { get; set; }
    }
     
    public class WebRtcConfig
    {
        public List<iceServer> iceServers { get; set; } 
        public iceTransportPolicyType iceTransportPolicy { get; set; }
    }
}
