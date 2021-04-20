using LYL.Common;
using LYL.Logic.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Logic.Machine
{

    class loginInfo
    {
        public string machineId { get; set; }
        public string machineName { get; set; }
        /// <summary>
        /// web,windows,ios phone,android phone
        /// </summary>
        public machineType machineType { get; set; }
        public string username { get; set; }
        public string pwd { get; set; }
    }

    public class UserLogic
    {
        public static bool Login(string username, string pwd, string machineName)
        {
            var localmachine = MachineLogic.localMachine();
            var logininf = new loginInfo
            {
                pwd = pwd,
                machineName = machineName,
                username = username,
                machineType = machineType.windows
            };
            if (localmachine.username == username)
            {
                logininf.machineId = localmachine.machineId;
            }

            var url = ServerAddrs.lylApiServerAddr + "api/user/login";
            var token = HTTPRuqest.LYLPost<string>(url, logininf);
            //
            var usermachine = AutoLogin(token);
            localmachine.machineName = usermachine.machineName;
            localmachine.username = username;
            localmachine.machinepwd = usermachine.machinePwd;
            localmachine.token = token;
            localmachine.machineId = usermachine.machineId;
            MachineLogic.RecordMachineInfo(localmachine);
            return true;
        }

        public static LYLUserMachineInfo AutoLogin(string token)
        {
            var url = ServerAddrs.lylApiServerAddr + "api/user/autoLogin";
            var headers = new Dictionary<string, string>();
            headers.Add("token", token);
            var usermachine = HTTPRuqest.LYLPost<LYLUserMachineInfo>(url, null, headers);
            return usermachine;
        }

        public static string requestRegisterUser(string username, string pwd, string email)
        {
            var url = ServerAddrs.lylApiServerAddr + "api/user/requestRegisterUser";
            var checkInfo = new EmailCheckInfo
            {
                username = username,
                pwd = pwd,
                email = email
            };
            var checkid = HTTPRuqest.LYLPost<string>(url, checkInfo);
            return checkid;
        }

        public static bool? checkRegisterUser(string checkId, string checkCode)
        {
            var url = ServerAddrs.lylApiServerAddr + "api/user/checkRegisterInfo";
            var checkInfo = new EmailCheckInfo
            {
                checkId = checkId,
                checkingCode = checkCode
            };
            return HTTPRuqest.LYLPost<bool?>(url, checkInfo);
        }

        public static string requestChangePwd(string username)
        {
            var url = ServerAddrs.lylApiServerAddr + "api/user/requestChangePwd";
            var checkInfo = new EmailCheckInfo
            {
                username = username
            };
            var checkid = HTTPRuqest.LYLPost<string>(url, checkInfo);
            return checkid;
        }

        public static bool? checkChangePwd(string checkId, string checkCode, string newPwd)
        {
            var url = ServerAddrs.lylApiServerAddr + "api/user/checkChangePwdInfo";
            var checkInfo = new EmailCheckInfo
            {
                checkId = checkId,
                checkingCode = checkCode,
                pwd = newPwd
            };
            return HTTPRuqest.LYLPost<bool?>(url, checkInfo);
        }


    }
}
