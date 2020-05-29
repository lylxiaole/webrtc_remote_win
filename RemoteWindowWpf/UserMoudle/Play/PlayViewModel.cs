using Dispath;
using Dispath.MoudleInterface;
using LYL.Common;
using LYL.Logic;
using LYL.Logic.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using UserMoudle.RemoteWindow.remoteControlLogic;
using WebrtcSDK_NET.AnyRTC_RTMP;
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.Play
{
    public class PlayViewModel
    {
        AnyRTCRtmp rtmpclient { get; set; }
        public PictureBox LocalImageControl { get; set; }

        public PlayViewModel()
        {
            this.rtmpclient = new AnyRTCRtmp();
        }

        public string StartPlay()
        {
            var key = PlayVideoLogic.ApplyPlay();
            var newguid = Guid.NewGuid().ToString();
            var rtmpurl = ServerAddrs.lylPlayServerAddr + "?key=" + key + "/" + newguid;
            this.rtmpclient.StartPushRtmp(rtmpurl, this.LocalImageControl.Handle); 
            return ServerAddrs.lylPlayServerAddr + "/" + newguid;
        }

        public void ClosePlay()
        {
            this.rtmpclient.Dispose();
            NavigationHelper.NavigatedToView("主界面");
        }

    }
}
