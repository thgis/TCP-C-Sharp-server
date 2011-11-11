using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class UserLoggedOnMessage : IComMessage
    {
        public string Name { get; set; }

        #region IComMessage Members

        public byte[] serialize()
        {
            throw new NotImplementedException();
        }

        public void deserialize(List<byte> data)
        {
            string[] msgs = Encoding.UTF8.GetString(data.ToArray()).Split(':');
            Name = msgs[1];
        }
        /// <summary>
        /// Returns messagetype
        /// </summary>
        public MessageType Id
        {
            get { return MessageType.USER; }
        }

        #endregion
    }
}
