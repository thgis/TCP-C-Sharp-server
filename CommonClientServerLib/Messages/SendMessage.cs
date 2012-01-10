using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class SendMessage : BaseMessage, IComMessage
    {
        public string message { get; set; }
        public string receiver { get; set; }
        public string sender { get; set; }
        public bool success { get; set; }

       

//-errorMessage:<message>
//-timeStamp:<long int> 


        #region IComMessage Members

        public MessageType type
        {
            get { return MessageType.SENDMESSAGE; }
        }
        #endregion
    }
}
