using AppModule.InterProcessComm;
using AppModule.NamedPipes;
using MouseKeyPlayback;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Common.WindowServiceManager
{
    public class pipeMessageData<T>
    {
        public pipeMessageType Type { get; set; }
        public T Data { get; set; }
    }

    public enum pipeMessageType
    {
        StartRemote = 1
    }


    public class pipeMessage
    {
        public bool isListening { get; set; } = false;
        public static string pipeName { get; set; } = "xxxoapsiakljkdshfjklsadhfasd";

        public void StartPipeServer()
        {
            isListening = true;
            ServerPipeConnection PipeConnection = new ServerPipeConnection(pipeName, 512, 512, 5000, false);
            Task.Run(() =>
            {
                while (isListening)
                {
                    PipeConnection.Disconnect();
                    PipeConnection.Connect();
                    try
                    {
                        string request = PipeConnection.Read();
                        onMessage?.Invoke(this, JsonConvert.DeserializeObject<pipeMessageData<object>>(request));
                    }
                    catch (Exception ex)
                    {
                    }
                }
                PipeConnection.Dispose();
            });
        }
         

        public void SendRequest<T>(pipeMessageData<T> request)
        {
            try
            {
                using (IInterProcessConnection clientConnection = new ClientPipeConnection(pipeName, "."))
                {
                    clientConnection.Connect();
                    clientConnection.Write(JsonConvert.SerializeObject(request));
                }
            }
            catch (Exception ex)
            {
            }
        }


        public event EventHandler<pipeMessageData<object>> onMessage;
    }
}
