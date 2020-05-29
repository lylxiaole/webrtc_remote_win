using LYL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Logic.Machine
{
    public class PlayVideoLogic
    {
        public static string ApplyPlay()
        {
            var url = ServerAddrs.lylApiServerAddr + "api/videoPlay/createPlay";
            var headers = new Dictionary<string, string>();
            headers.Add("token", MachineLogic.localMachine().token);
            return HTTPRuqest.LYLPost<string>(url,null, headers);
        }
    }
}
