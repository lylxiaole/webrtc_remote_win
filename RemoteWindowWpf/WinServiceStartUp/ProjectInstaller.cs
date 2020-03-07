using LYL.Common.WindowServiceManager;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace WinServiceStartUp
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
          
            InitializeComponent();

            try
            {
                serviceInstaller1.DisplayName = WinServiceDescription.WinServiceDisplayName;
                serviceInstaller1.ServiceName = WinServiceDescription.WinServiceName;
                serviceInstaller1.StartType = ServiceStartMode.Automatic;
                serviceInstaller1.DelayedAutoStart = false;
             
            }
            catch (Exception)
            { 
            }
        }



        protected override void OnBeforeInstall(IDictionary savedState)
        {
          
          
            base.OnBeforeInstall(savedState);
           
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            ManagementBaseObject inPar = null;
            ManagementClass mc = new ManagementClass("Win32_Service");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo["Name"].ToString() == "服务名")
                {
                    inPar = mo.GetMethodParameters("Change");
                    inPar["DesktopInteract"] = true;
                    mo.InvokeMethod("Change", inPar, null);
                }
            }
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
        }
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            //Auot start service after the installation is completed
            ServiceController sc = new ServiceController("LYLRemote");
            if (sc.Status.Equals(ServiceControllerStatus.Stopped))
            {
                sc.Start();
            }
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            try
            {

                System.Management.ManagementObject myService = new System.Management.ManagementObject(
                    string.Format("Win32_Service.Name='{0}'", this.serviceInstaller1.ServiceName));
                System.Management.ManagementBaseObject changeMethod = myService.GetMethodParameters("Change");
                changeMethod["DesktopInteract"] = true;
                System.Management.ManagementBaseObject OutParam = myService.InvokeMethod("Change", changeMethod, null);
            }
            catch (Exception)
            {
            }
            base.OnAfterInstall(savedState);

        }

  


    }
}
