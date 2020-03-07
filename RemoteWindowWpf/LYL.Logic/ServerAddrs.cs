using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Logic
{
    public class ServerAddrs
    {

        public static string lylApiServerAddr
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["lylApiServerAddr"];
            } 
        }

        public static string lylWebSocketAddr
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["lylWebSocketAddr"];
            }
        }

        public static string lylRtcServerAddr
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["lylRtcServerAddr"];
            }
        }
    }
}
