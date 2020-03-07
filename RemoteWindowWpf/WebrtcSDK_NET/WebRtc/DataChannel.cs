using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebrtcSDK_NET.WebRtc
{
    public class DataChannel
    {
        private onChannelStateChanged_Callback _onChannelStateChanged_Callback_Handle;
        private onChannelStateChanged_Callback onChannelStateChanged_Callback_Handle
        {
            get => _onChannelStateChanged_Callback_Handle = (RTCDataChannelState state, string _channelId) =>
              {
                  this.onChannelStateChanged?.Invoke(this, state);
              };
        }

        private onChannelMessage_Callback _onChannelMessage_Callback_Handle;
        private onChannelMessage_Callback onChannelMessage_Callback_Handle
        {
            get => _onChannelMessage_Callback_Handle = (string buffer, Int32 length, bool binary, string _channelId) =>
              {
                  var message = new ChannelMessage
                  {
                      binary = binary,
                      buffer = buffer.Substring(0, length),
                      length = length
                  };
                  this.onChannelMessage?.Invoke(this, message);
              };
        }
         
        private int _connectionAddr { get; set; }

        public string label { get; set; }
 
        public DataChannel(string channellabel, int connectionAddr)
        {
            this.label = channellabel;
            this._connectionAddr = connectionAddr;
        }

        internal void StartListenEvent()
        {
            libwebrtcNET.SetDataChannelEvent(this.onChannelStateChanged_Callback_Handle, this.onChannelMessage_Callback_Handle, this.label, this._connectionAddr);
        }

        public void SendChannelData(string data)
        {
            libwebrtcNET.SendChannelData(this.label, data?.Trim(), this._connectionAddr);
        }

        public event EventHandler<RTCDataChannelState> onChannelStateChanged;
        public event EventHandler<ChannelMessage> onChannelMessage;
    }
}
