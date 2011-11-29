using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class TextMessage : IComMessage
    {
        #region IComMessage Members

        public string Text { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }

        public MessageType Id
        {
            get { return MessageType.TEXT; }
        }

        #endregion
    }
}
