using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class PublishMessage : BaseMessage, IComMessage
    {
        public string message { get; set; }
        public string receiver { get; set; }
        public string sender { get; set; }

        #region IComMessage Members

        public MessageType type
        {
            get { return MessageType.PUBLISHMESSAGE; }
        }
        #endregion
    }
}
