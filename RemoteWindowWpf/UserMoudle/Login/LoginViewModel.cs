
using Controls.Dialogs;
using Dispath;
using Dispath.MoudleInterface;
using LYL.Logic.Machine;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMoudle
{
    public class LoginViewModel : ViewModelBase
    {
        public string userName { get; set; }
        public string pwd { get; set; }

        private DelegateCommand _loginCommand => new DelegateCommand(this.loginMethod);
        public DelegateCommand loginCommand { get { return this._loginCommand; } }
        public void loginMethod(object param)
        {
            if (UserLogic.Login(this.userName, pwd, Environment.MachineName))
            {
                var machineInfo = MachineLogic.localMachine();
                var msg= "登录成功,机器ID:" + machineInfo.machineId + ",机器密码:" + machineInfo.machinepwd;
                this.ShowMessageBoxSuccess(msg, "成功", System.Windows.MessageBoxButton.OK); 
                NavigationHelper.NavigatedToView("主界面");
            }
        }


    }
}
