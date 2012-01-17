using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using CommonClientServerLib.Messages;

namespace CommonClientServerLib
{
    public class MessageHandler
    {
        public static IComMessage DecodePacketJson(List<byte> data)
        {
            JavaScriptSerializer JSR = new JavaScriptSerializer();
            IComMessage msg = null;
            string msgStr = Encoding.UTF8.GetString(data.ToArray());

            Dictionary<string, object> test = (Dictionary<string, object>)JSR.DeserializeObject(msgStr);

            MessageType msgType = MessageType.NOMATCHINGTYPE;

            if (test.ContainsKey("type"))
                msgType = (MessageType)int.Parse(test["type"].ToString());

            switch (msgType)
            {
                case MessageType.USERLOGON:
                    msg = JSR.Deserialize<UserLogOn>(msgStr);
                    break;
                case MessageType.SENDMESSAGE:
                    msg = JSR.Deserialize<SendMessage>(msgStr);
                    break;
                case MessageType.GETONLINEUSERS:
                    msg = JSR.Deserialize<GetOnlineUsers>(msgStr);
                    break;
                case MessageType.PUBLISHMESSAGE:
                    msg = JSR.Deserialize<PublishMessage>(msgStr);
                    break;
                default:
                    break;
            }
            return msg;
        }

        public static byte[] EncodePacket(IComMessage iComMessage)
        {
            JavaScriptSerializer JSR = new JavaScriptSerializer();
            string msg = JSR.Serialize(iComMessage);

            List<byte> list = new List<byte>();
            list.Add(0x02);
            list.AddRange(Encoding.ASCII.GetBytes(msg));
            list.Add(0x10);
            list.Add(0x03);

            return list.ToArray();
        }
    }

    public enum MessageType
    {        
        SENDMESSAGE = 5,
        PUBLISHMESSAGE = 4,
        NEWUSERONLINE = 3,
        GETONLINEUSERS = 2,
        USERLOGON = 1,
        NOMATCHINGTYPE = 0
    }
}
