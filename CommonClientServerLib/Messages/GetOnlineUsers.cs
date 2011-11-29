using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class GetOnlineUsers : IComMessage
    {
        public List<string> Users { get; set; }

        #region IComMessage Members

        public MessageType Id
        {
            get { return MessageType.GETONLINEPEOPLE; }
        }

        #endregion
    }
}
