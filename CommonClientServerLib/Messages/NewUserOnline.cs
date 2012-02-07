using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class NewUserOnline : BaseMessage, IComMessage
    {
        public string userName { get; set; }

        #region IComMessage Members

        public MessageType type
        {
            get { return MessageType.NEWUSERONLINE; }
        }

        #endregion
    }
}
