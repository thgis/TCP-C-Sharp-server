using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class UserLogOn : BaseMessage, IComMessage
    {
        public string userName { get; set; }
        public bool success { get; set; }
        public int id { get; set; }
        #region IComMessage Members

        public MessageType type
        {
            get { return MessageType.USERLOGON; }
        }

        #endregion
    }
}
