using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class UserLogOn : IComMessage
    {
        public string UserName { get; set; }

        #region IComMessage Members

        public MessageType Id
        {
            get { return MessageType.USER; }
        }

        #endregion
    }
}
