using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class MessageEvent
    {
        private IComMessage message;
        private ClientInfo client;

        public MessageEvent(IComMessage message, ClientInfo client)
        {
            this.message = message;
            this.client = client;
        }

        public IComMessage GetMessage
        {
            get
            {
                return message;
            }
        }

        public ClientInfo ClientID
        {
            get
            {
                return client;
            }
        }
    }
}
