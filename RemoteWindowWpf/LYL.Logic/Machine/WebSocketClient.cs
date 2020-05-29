using LYL.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace LYL.Logic.Machine
{
    [AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MessageTypeAttribute : Attribute
    {
        public msgType msgType { get; set; }

        public MessageTypeAttribute(msgType msgType_)
        {
            this.msgType = msgType_;
        }

    }

    public class WebSocketClient
    {
        public event EventHandler<websocketMsgTemp<object>> OnMessage;
        public event EventHandler<CloseEventArgs> OnClose;
        public event EventHandler<ErrorEventArgs> OnError;
        public WebSocketState state
        {
            get
            {
                if (wsclient == null)
                {
                    return WebSocketState.Closed;
                }
                return wsclient.ReadyState;
            }
        }

        public event EventHandler OnOpen;
        private static WebSocket wsclient = null;


        public WebSocketClient()
        {
            if (wsclient == null)
            {
                var machineInfo = MachineLogic.localMachine();
                wsclient = new WebSocket(ServerAddrs.lylWebSocketAddr + "?token=" + machineInfo.token); 
            }
            wsclient.EmitOnPing = true;
            wsclient.OnMessage += Wsclient_OnMessage;
            wsclient.OnClose += Wsclient_OnClose;
            wsclient.OnError += Wsclient_OnError;
            wsclient.OnOpen += Wsclient_OnOpen;
        }

        public bool isConnecting { get; set; } = false;

        public void StartClient()
        {
            this.isConnecting = true;
         
            wsclient.Connect();
        }

        private void Wsclient_OnOpen(object sender, EventArgs e)
        {
            this.isConnecting = false;
            this.OnOpen?.Invoke(this, e);
        }

        private void Wsclient_OnError(object sender, ErrorEventArgs e)
        {
            this.OnError?.Invoke(this, e);
        }

        private void Wsclient_OnClose(object sender, CloseEventArgs e)
        {
            if (this.isConnecting == false)
            {
                this.OnClose?.Invoke(this, e);
            }
        }

        private void Wsclient_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText || e.IsBinary)
            {
                var data = JsonConvert.DeserializeObject<websocketMsgTemp<object>>(e.Data);
                this.OnMessage?.Invoke(this, data);
            }
        }
        ///***************************************************
        ///
        public static void SendMessage(string remoteMachineId, object content, msgType msgtype)
        {
            websocketMsgTemp<object> data = new websocketMsgTemp<object>();
            data.sendMachineId = MachineLogic.localMachine().machineId;
            data.receiverMachineId = remoteMachineId;
            data.msgType = msgtype;
            data.content = content;
            wsclient.Send(JsonConvert.SerializeObject(data));
        }


        ///**************************************************
        ///
        public void DealMessage(object messageHandleClass, websocketMsgTemp<object> e)
        {
            foreach (var method in messageHandleClass.GetType().GetMethods())
            {
                var attrs = method.GetCustomAttributes(typeof(MessageTypeAttribute), true) as MessageTypeAttribute[];
                if (attrs.Length < 1)
                {
                    continue;
                }

                var find = attrs.FirstOrDefault(o => o.msgType == e.msgType);
                if (find == null)
                {
                    continue;
                }

                var param = method.GetParameters()[0]; 
                var data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(e), param.ParameterType); 
                method.Invoke(messageHandleClass, new object[] { data });
                break;

            }
        }

    }
}
