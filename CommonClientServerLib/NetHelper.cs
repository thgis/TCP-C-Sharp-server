using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace CommonClientServerLib
{
    public static class NetHelper
    {

        public static String GetIP()
        {
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);


            // Grab the first IP addresses
            String IPStr = "";
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                IPStr = ipaddress.ToString();
                return IPStr;
            }
            return IPStr;
        }
    }
}
