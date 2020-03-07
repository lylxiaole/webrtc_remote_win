using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Data.Models
{
    public class MachineInfo : ModelBase
    {
        public string token { get; set; }
        public string username { get; set; }
        public string machineId { get; set; }
        public string machineName { get; set; }
        public string machinepwd { get; set; }
    }
}
