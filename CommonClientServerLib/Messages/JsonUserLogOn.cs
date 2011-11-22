using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class JsonUserLogOn : IComMessage
    {
        public string UserName { get; set; }

        #region IComMessage Members

        public byte[] serialize()
        {
            throw new NotImplementedException();
        }

        public void deserialize(List<byte> data)
        {
            throw new NotImplementedException();
        }

        public MessageType Id
        {
            get { return MessageType.USER; }
        }

        #endregion
    }
}
