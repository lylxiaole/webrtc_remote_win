using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WebrtcSDK_NET.WebRtc
{
    public class MediaStream
    {
        private IntPtr _connectionAddr { get; set; }
        public List<Track> tracks { get; set; } = new List<Track>();
         
        public IntPtr connectionAddr
        {
            get
            {
                return _connectionAddr;
            }
        }
        public string streamId { get; set; }
        public MediaStream(string streamId_, IntPtr connectionAddr)
        {
            this.streamId = streamId_;
            this._connectionAddr = connectionAddr;
        }

        public void AddTrack(Track track)
        {
            this.tracks.Add(track);
            if (track.trackType == "video")
            {
                libwebrtcNET.SetVideoTrackRGBAHandle(this.onRemoteVideoRgba_Callback_Handle, track.trackId, this._connectionAddr);
            }
            else
            {
                //libwebrtcNET.SetAudioTrackHandle(this.onRemoteFrame_Callback_Handle, track.trackId, this._connectionAddr);
            }
        }
         
        public void Close()
        {
            libwebrtcNET.RemoveStream(streamId, this._connectionAddr);
        }


         

        private onFrame_Callback _onRemoteVideoRgba_Callback_Handle;
        private onFrame_Callback onRemoteVideoRgba_Callback_Handle
        {
            get => _onRemoteVideoRgba_Callback_Handle = (IntPtr rgbImg, Int32 buffer_size, Int32 width, Int32 height, string vedioTrackId) =>
            {
                if (this.onRemoteVideoFrame != null)
                {
                    byte[] data = new byte[buffer_size];
                    Marshal.Copy(rgbImg, data, 0, buffer_size);
                    var lframe = new VedioFrame { rgbImg = data, width = width, height = height, vedioTrackId = vedioTrackId };
                    this.onRemoteVideoFrame?.Invoke(this, lframe);
                }
            };
        }

        public event EventHandler<VedioFrame> onRemoteVideoFrame;
    }
}
