using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dispath.MoudleInterface
{
    public class ViewAttribute : Attribute
    {
        public ViewAttribute() { }

        public string MoudleName { get; set; }
        public string MoudleImage { get; set; }
    }
}
