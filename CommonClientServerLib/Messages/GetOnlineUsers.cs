using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class GetOnlineUsers : BaseMessage, IComMessage
    {
        public List<string> users { get; set; }

        #region IComMessage Members

        public MessageType type
        {
            get { return MessageType.GETONLINEUSERS; }
        }

        #endregion
    }
}
