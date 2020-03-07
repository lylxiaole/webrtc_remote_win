
using EncodeousCommon.Sys.Windows;
using LYL.Common;
using LYL.Common.WindowServiceManager;
using MouseKeyPlayback;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinServiceStartUp
{
    public partial class WinStartUp : ServiceBase
    {
        public bool watchProcessRunning { get; set; } = false;
        public object lockobj = new object();

        public WinStartUp()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            //**********************************************
            pipeMessage pipes = new pipeMessage();
            pipes.onMessage += Pipes_onMessage;
            pipes.StartPipeServer();
            //******定时检测客户端有没有启动****************************************  
            //Task.Run(() =>
            //{
            //    if (this.watchProcessRunning == true)
            //    {
            //        return;
            //    }
            //    this.watchProcessRunning = true;
            //    while (watchProcessRunning)
            //    { 
            //            this.StartupRemoteWindow(); 
            //        Thread.Sleep(60000);
            //    }
            //});
            this.StartupRemoteWindow();
        }
         
        protected override void OnStop()
        {
            base.OnStop(); 
            this.StopRemoteWindow(); 
        }

        protected override void OnPause()
        {
            base.OnPause();
            lock (lockobj)
            {
                StopRemoteWindow();
            }
        }

        private void Pipes_onMessage(object sender, pipeMessageData<object> e)
        {
            try
            {
                switch (e.Type)
                {
                    case pipeMessageType.StartRemote:
                        this.StartupRemoteWindow();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
        }
         
        private void StartupRemoteWindow()
        {
            lock (lockobj)
            {
                var findprocess = Process.GetProcesses().FirstOrDefault(o => o.ProcessName == WinServiceDescription.WinServiceName);
                try
                {
                    if (findprocess == null)
                    {
                        var remoteExeWindow = System.AppDomain.CurrentDomain.BaseDirectory + @"remoteWindow\LYLRemote.exe";
                        if (findprocess == null)
                        {
                            //ApplicationLoader.PROCESS_INFORMATION proc;
                            //ApplicationLoader.StartProcessAndBypassUAC(remoteExeWindow, out proc); 
                            ProcessManager.PROCESS_INFORMATION proc;
                            Sessions.StartProcessInActiveSession(remoteExeWindow, out proc);
                            //Sessions.StartProcessInSession(remoteExeWindow, Sessions.GetActiveSessionID(), out proc);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        private void StopRemoteWindow()
        {
            lock (lockobj)
            {
                var findprocesses = Process.GetProcesses().Where(o => o.ProcessName == WinServiceDescription.WinServiceName);
                foreach (var findprocess in findprocesses)
                {
                    try
                    {
                        findprocess.Kill();
                        findprocess.WaitForExit();
                        findprocess.Dispose();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
         
    }
}
