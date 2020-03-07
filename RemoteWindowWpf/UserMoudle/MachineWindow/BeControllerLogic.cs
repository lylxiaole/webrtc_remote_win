using Controls.Dialogs;
using LYL.Common;
using LYL.Logic.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UserMoudle.RemoteWindow;
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.MachineWindow
{
    public class BeControllerLogic
    {
        /// <summary>
        /// 循环检测peerconnection链接的标识
        /// </summary>
        public bool isChecking { get; set; } = true;

        /// <summary>
        /// 被控制窗体
        /// </summary>
        BeControllWindow beConWindow { get; set; }
        /// <summary>
        /// 被控制窗体关闭是否需要询问
        /// </summary>
        public bool closeNeedConfirm { get; set; } = true;

        public List<RemoteBeingControlled> beControlleds { get; set; } = new List<RemoteBeingControlled>();

        public BeControllerLogic()
        {
            Task.Run(() =>
            {
                while (this.isChecking)
                {
                    Thread.Sleep(10000);
                    checkNoUseConnection();
                }
            });
        }

        ~BeControllerLogic()
        {
            this.isChecking = false;
        }

        private void closeBecontrolled(Guid Id)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var _becontrolled = beControlleds.FirstOrDefault(o => o.Id == Id);
                if (_becontrolled != null)
                {
                    _becontrolled.CloseConnection();
                    _becontrolled.onCloseEvent -= BeControlled_onCloseEvent;
                    beControlleds.Remove(_becontrolled);
                    this.beConWindow = null;
                }
            });
        }

        private void checkNoUseConnection()
        {
            try
            {
                this.beControlleds.ForEach(o =>
                {
                    if (o.State == RTCIceConnectionState.RTCIceConnectionStateFailed || o.State == RTCIceConnectionState.RTCIceConnectionStateDisconnected || o.State == RTCIceConnectionState.RTCIceConnectionStateClosed)
                    {
                        o.CloseConnection();
                    }
                });
                this.beControlleds.RemoveAll((o) =>
                {
                    if (o.State == RTCIceConnectionState.RTCIceConnectionStateFailed || o.State == RTCIceConnectionState.RTCIceConnectionStateDisconnected || o.State == RTCIceConnectionState.RTCIceConnectionStateClosed)
                    {
                        return true;
                    }
                    return false;
                });
            }
            catch (Exception)
            {
            }
        }

        public bool CanConnect
        {
            get
            {
                if (this.beControlleds.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void ReciveRemoteConnection(string machineId, SdpInfo remoteOfferInfo)
        {
            if (this.CanConnect == true)
            {
                var newbeControlled = new RemoteBeingControlled();
                newbeControlled.onCloseEvent += BeControlled_onCloseEvent;
                newbeControlled.ReciveRemoteConnection(machineId, remoteOfferInfo);
                //*****************************************************
                Application.Current.MainWindow.Close();
                //******************************************************
                this.beControlleds.Add(newbeControlled);
                this.beConWindow = new BeControllWindow(newbeControlled);
                this.closeNeedConfirm = true;
                this.beConWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.beConWindow.Show();
                this.beConWindow.Closing += BeConWindow_Closing;
            }
        }

        private void BeConWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.closeNeedConfirm == true)
            {
                MessageBoxResult result = MessageBox.Show("确定断开链接？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.closeBecontrolled(this.beConWindow.becontrolContext.Id);
                }
            }
            else
            {
                this.closeBecontrolled(this.beConWindow.becontrolContext.Id);
            }
        }

        private void BeControlled_onCloseEvent(object sender, Guid Id)
        {
            this.closeNeedConfirm = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.beConWindow.Close();
            });
        }

        public void onRemoteClientSdpCompleted(string machineId)
        {
            var find = this.beControlleds.FirstOrDefault(o => o.remoteMachine.machineId == machineId);
            if (find != null)
            {
                find.onRemoteClientSdpCompleted(machineId);
            }
        }

        public void AddRemoteIceCandite(string mchineId, IceCandidate iceCandidate)
        {
            var find = this.beControlleds.FirstOrDefault(o => o.remoteMachine.machineId == mchineId);
            if (find != null)
            {
                find.AddRemoteCandidate(mchineId, iceCandidate);
            }
        }

    }
}
