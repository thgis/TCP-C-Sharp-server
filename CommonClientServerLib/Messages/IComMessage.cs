using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public interface IComMessage
    {
        /// <summary>
        /// Property to get the ID.
        /// </summary>
        MessageType type { get; }
    }
}
