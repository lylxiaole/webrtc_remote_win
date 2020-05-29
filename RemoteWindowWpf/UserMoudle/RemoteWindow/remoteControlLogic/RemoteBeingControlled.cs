
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Controls.Dialogs;
using EncodeousCommon.Sys.Windows;
using LYL.Common;
using LYL.Common.WindowServiceManager;
using LYL.Logic.Machine;
using MouseKeyPlayback; 
using Newtonsoft.Json; 
using UserMoudle.RemoteWindow.remoteControlLogic;
using WebrtcSDK_NET.WebRtc;


namespace UserMoudle.RemoteWindow
{
    public class RemoteBeingControlled : RemoteControlBase
    {

        public RemoteBeingControlled( ) : base()
        {
            this.IsDesktop = true;
        }

        public bool ReciveRemoteConnection(string machineId, SdpInfo remoteOfferInfo)
        {
            this.remoteMachine = MachineLogic.GetMachineById(machineId);
            this.InitlizeConnetion();
            var answerInfo = this.PeerConnection.SetRemoteDescription(remoteOfferInfo.sdp, remoteOfferInfo.type).Result;
            if (answerInfo == null)
            {
                this.showInfo("流媒体设备启动失败");
                return false;
            }

            WebSocketClient.SendMessage(machineId, answerInfo, msgType.client_onCallee_CreateAnswer);
            return true;
        }

        public void onRemoteClientSdpCompleted(string machineId)
        {
            WebSocketClient.SendMessage(machineId, "", msgType.client_onCallee_SetRemoteSdpCompleted); 
        }

        protected override void PeerConnection_onNewDataChannel(object sender, DataChannel e)
        {
            base.PeerConnection_onNewDataChannel(sender, e);
            if (e.label == nameof(ChannelName.fileChannel))
            {
                FileChannelMessageDeal.RegisterFileChannel(e);
            }
            else
            {
                e.onChannelMessage += E_onChannelMessage;
                e.onChannelStateChanged += E_onChannelStateChanged;
            }
        }

        protected override void onIceCandidateEvent(object sender, IceCandidate e)
        {
            if (this.remoteMachine == null)
            {
                return;
            }
            WebSocketClient.SendMessage(this.remoteMachine.machineId, e, msgType.client_onCallee_CreateIceCandite);
        }

        private void E_onChannelStateChanged(object sender, RTCDataChannelState e)
        {
        }
        private void E_onChannelMessage(object sender, ChannelMessage e)
        {
            var channel = sender as DataChannel;
            switch (channel.label)
            {
                case nameof(ChannelName.fileChannel):
                    {
                    }
                    break;
                case nameof(ChannelName.keyMouseChannel):
                    {
                        dealKeyboardMessage(e);
                    }
                    break;
                default:
                    break;
            }

        }

        #region 处理鼠标键盘事件 

        private void dealKeyboardMessage(ChannelMessage e)
        { 
            try
            {
                Desktop.SwitchToInputDesktop();
                List<Record> events = JsonConvert.DeserializeObject<List<Record>>(e.buffer);
                foreach (var eve in events)
                {
                    if (eve.Type == Constants.MOUSE)
                    {
                        var mouselocation = eve.EventMouse.Location;
                        var mouseXPoint = Convert.ToInt32(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * mouselocation.X);
                        var mouseYPoint = Convert.ToInt32(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * mouselocation.Y);
                        eve.EventMouse.Location = new CursorPoint(mouseXPoint, mouseYPoint);
                        PlaybackMouse(eve);
                    }
                    else if (eve.Type == Constants.KEYBOARD)
                    {
                        PlaybackKeyboard(eve);
                    }
                }
            }
            catch (Exception)
            { 
            }
        }

        pipeMessage pipeClient = new pipeMessage(); 
        private void PlaybackMouse(Record record)
        {
            //pipeClient.SendRequest(record);
            //return;  
            CursorPoint newPos = record.EventMouse.Location;
            MouseHook.MouseEvents mEvent = record.EventMouse.Action;
            MouseUtils.PerformMouseEvent(mEvent, newPos);
        }

        private void PlaybackKeyboard(Record record)
        {
            //pipeClient.SendRequest(record);
            //return;  
            Keys key = record.EventKey.Key;
            string action = record.EventKey.Action;
            KeyboardUtils.PerformKeyEvent(key, action);
        }
        #endregion


    }
}
