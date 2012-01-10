using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public abstract class BaseMessage
    {
        public string errorMessage { get; set; }
        public long timeStamp { get; set; }
    }
}
