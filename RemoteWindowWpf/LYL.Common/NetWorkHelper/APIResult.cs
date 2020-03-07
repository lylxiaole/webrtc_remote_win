using System;
using System.Collections.Generic;
using System.Text;

namespace LYL.Common
{
    public class APIResult<T>
    {
        public string errMsg { get; set; }
        public int errCode { get; set; }
        public bool success { get; set; }
        public T data { get; set; }
    }
}
