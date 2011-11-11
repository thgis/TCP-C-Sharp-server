using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib
{
    public abstract class BaseEventArgs
    {

    }

    public class ConnectionChangedEventArgs : BaseEventArgs
    {
        public ClientInfo ClientInfo { get; set; }
    }

    public class CompletePacketReceivedArgs : BaseEventArgs
    {
        public List<byte> Data { get; set; }

        public CompletePacketReceivedArgs(List<byte> data)
        {
            Data = data;
        }
    }
}
