using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebrtcSDK_NET.WebRtc
{
    public class MediaStream
    {
        private int _connectionAddr { get; set; }
        public List<Track> tracks { get; set; } = new List<Track>();

        public string streamLabel { get; set; }
        public MediaStream(string streamLabel,int connectionAddr)
        {
            this.streamLabel = streamLabel;
            this._connectionAddr = connectionAddr;
        }

        public void AddTrack(Track track)
        {
            this.tracks.Add(track);
            if (track.trackType == "video")
            {
                libwebrtcNET.SetListenonRemoteFrame(this.onRemoteFrame_Callback_Handle, track.trackId, this._connectionAddr);
            }
        }

        public void Close()
        {
            libwebrtcNET.RemoveStream(streamLabel, this._connectionAddr); 
        }


        private onFrame_Callback _onRemoteFrame_Callback_Handle;
        private onFrame_Callback onRemoteFrame_Callback_Handle
        {
            get => _onRemoteFrame_Callback_Handle = (IntPtr rgbImg, Int32 buffer_size, Int32 width, Int32 height, string vedioTrackId) =>
            {
                if (this.onRemoteFrame != null)
                {
                    byte[] data = new byte[buffer_size];
                    Marshal.Copy(rgbImg, data, 0, buffer_size);
                    var lframe = new VedioFrame { rgbImg = data, width = width, height = height, vedioTrackId = vedioTrackId };
                    this.onRemoteFrame?.Invoke(this, lframe);
                }
            };
        }

        public event EventHandler<VedioFrame> onRemoteFrame;
    }
}
