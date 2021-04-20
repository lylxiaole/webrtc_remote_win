using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Logic.Types
{
    public enum machineType
    {
        web = 1,
        windows = 2,
        ios_phone = 3,
        android_phone = 4
    }
    public class LYLUserMachineInfo
    {
        public string id { get; set; }
        public string machineId { get; set; }
        public string username { get; set; }
        public machineType machineType { get; set; }
        public string machineName { get; set; }
        public string machinePwd { get; set; }
    }
}
