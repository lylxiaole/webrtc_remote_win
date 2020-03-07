using LYL.Logic.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserMoudle.RemoteWindow;
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.MachineWindow
{
    public class ControllerLogic
    {

        ControllerWindowView controllerWindow { get; set; }
        public ControllerLogic()
        {
        }

        public void ConnectMachine(string mchineId)
        {
            if (this.controllerWindow == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.controllerWindow = new ControllerWindowView();
                    this.controllerWindow.Closed += ControllerWindow_Closed;
                    this.controllerWindow.Show();
                });
            }
            this.controllerWindow.ConnectMachine(mchineId);
        }

        private void ControllerWindow_Closed(object sender, EventArgs e)
        {
            this.controllerWindow = null;
        }


        /// <summary>
        /// 主动连接对方成功，接受对方发来的answer
        /// </summary>
        /// <param name="mchineId"></param>
        /// <param name="offerinfo"></param>
        public void SetRemoteAnswer(string mchineId, SdpInfo remoteAnswer)
        {
            this.controllerWindow.SetRemoteAnswer(mchineId, remoteAnswer);
        }

        public void onRemoteClientSdpCompleted(string machineId)
        {
            this.controllerWindow.onRemoteClientSdpCompleted(machineId);
        }

        /// <summary>
        /// 接收到对方发来的IceCandite，进行处理
        /// </summary>
        /// <param name="mchineId"></param>
        /// <param name="iceCandidate"></param>
        public void AddRemoteIceCandite(string mchineId, IceCandidate iceCandidate)
        {
            this.controllerWindow.AddRemoteIceCandite(mchineId, iceCandidate);
        }
    }
}
