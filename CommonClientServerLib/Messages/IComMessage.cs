using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public interface IComMessage
    {
        /// <summary>
        /// Method used to serialize a packet.
        /// </summary>
        /// <returns>Byte array.</returns>
        byte[] serialize();
        /// <summary>
        /// Method used to deserialize a message from a byte array.
        /// </summary>
        /// <param name="data"></param>
        void deserialize(List<byte> data);
        /// <summary>
        /// Property to get the ID.
        /// </summary>
        MessageType Id { get; }
    }
}
