using LYL.Common.WindowServiceManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace StartApp
{
    class Program
    {
        static void Main(string[] args)
        {
            StartUpService(); 
        }

        private static void StartUpService()
        {
            try
            {
                pipeMessageData<object> message = new pipeMessageData<object>();
                message.Type = pipeMessageType.StartRemote;
                pipeMessage pipe = new pipeMessage();
                pipe.SendRequest(message);
            }
            catch (Exception)
            {
            } 
        } 
    }
}
