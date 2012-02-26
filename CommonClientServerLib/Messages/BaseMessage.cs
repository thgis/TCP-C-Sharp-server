using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib.Messages
{
    public class BaseMessage
    {
        public BaseMessage()
        {
            timeStamp = GetTimeStamp();
        }
        public string errorMessage { get; set; }
        public long timeStamp { get; set; }


        public static long GetTimeStamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}
