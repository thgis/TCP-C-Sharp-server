using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib
{
    public class ClientInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }


        public override string ToString()
        {
            return "SocketID: " + ID.ToString() + " with Name: " + Name;
        }
    }

}
