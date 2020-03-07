using LYL.Common;
using LYL.Data;
using LYL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Logic.Machine
{
    public class MachineLogic
    {
        public static MachineInfo localMachine()
        {
            using (var dbcontext = new SqliteDbContext())
            {
                var find = dbcontext.MachineInfos.FirstOrDefault();
                if (find == null)
                {
                    find = new MachineInfo();
                    dbcontext.InsertOrReplace(find);
                    find = dbcontext.MachineInfos.FirstOrDefault();
                }
                return find;
            }
        }


        public static void RecordMachineInfo(MachineInfo machineinfo)
        {
            var localmachine = localMachine();
            localmachine.username = machineinfo.username;
            localmachine.machineId = machineinfo.machineId;
            localmachine.machineName = machineinfo.machineName;
            localmachine.machinepwd = machineinfo.machinepwd;
            localmachine.token = machineinfo.token;

            using (var dbcontext = new SqliteDbContext())
            {
                dbcontext.Update(localmachine);
            }
        }

        public static MachineInfo GetMachineById(string machineId)
        {
            var url = ServerAddrs.lylApiServerAddr + "api/machine/getMachaineById";
            var rparams = new Dictionary<string, string>();
            rparams.Add("machineId", machineId);
         

            var headers = new Dictionary<string, string>();
            headers.Add("token", localMachine().token);
            return HTTPRuqest.LYLGet<MachineInfo>(url, rparams, headers);
        }


        public static bool ChangeMachineName(string machineId, string machineName)
        {
            var url = ServerAddrs.lylApiServerAddr + "api/machine/changeMachineName";
            MachineInfo machineinfo = new MachineInfo { machineId = machineId, machineName = machineName };
        

            var headers = new Dictionary<string, string>();
            headers.Add("token", localMachine().token); 
            var res= HTTPRuqest.LYLPost<bool>(url, machineinfo, headers);

            if(res==true)
            {
                var localmachineInfo = localMachine();
                localmachineInfo.machineName = machineName;
                RecordMachineInfo(localmachineInfo);
            } 
            return res;
        }
    }
}
