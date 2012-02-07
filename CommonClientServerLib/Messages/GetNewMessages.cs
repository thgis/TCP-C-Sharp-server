using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    class GetNewMessages : BaseMessage, IComMessage
    {
        public string receiver { get; set; }
        public long lastSeenTimeStamp { get; set; }

        public MessageType type
        {
            get { return MessageType.GETNEWMESSAGES; }
        }
    }
}
