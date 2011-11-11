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

        public byte[] serialize()
        {
            throw new NotImplementedException();
        }

        public void deserialize(List<byte> data)
        {
            Text = Encoding.UTF8.GetString(data.ToArray());
        }

        public MessageType Id
        {
            get { return MessageType.TEXT; }
        }

        #endregion
    }
}
