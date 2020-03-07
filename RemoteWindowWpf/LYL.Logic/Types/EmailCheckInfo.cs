using System;
using System.Collections.Generic;
using System.Text;

namespace LYL.Logic.Types
{
    public enum checkType
    {
        register = 0,
        changePwd = 1

    }
    public class EmailCheckInfo 
    {
        public string checkId { get; set; }
        public string username { get; set; }
        public string pwd { get; set; }

        public string email { get; set; }

        public checkType type { get; set; }
        public string checkingCode { get; set; }

        public DateTime outerTime { get; set; }
    }
}
