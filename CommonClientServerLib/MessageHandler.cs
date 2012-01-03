using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using CommonClientServerLib.Messages;

namespace CommonClientServerLib
{

    /// <summary>
    /// Delegate to a method used when a message is received.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public delegate void MessageReceivedHandler(object sender, MessageEvent message);

    public class MessageHandler
    {
        IComMessage msg = null;
        /// <summary>
        /// Event that is raised when a Message is received.
        /// </summary>
        public event MessageReceivedHandler MessageReceived;
        JavaScriptSerializer JSR;

        public MessageHandler()
        {
            JSR = new JavaScriptSerializer();
        }

        public void DecodePacketJson(List<byte> data, ClientInfo clientInfo)
        {
            string msgStr = Encoding.ASCII.GetString(data.ToArray());

            Dictionary<string, object> test = (Dictionary<string, object>)JSR.DeserializeObject(msgStr);

            MessageType msgType = MessageType.NOMATCHINGTYPE;

            if (test.ContainsKey("type"))
                msgType = (MessageType)int.Parse(test["type"].ToString());

            switch (msgType)
            {
                case MessageType.USERLOGON:
                    msg = JSR.Deserialize<UserLogOn>(msgStr);
                    break;
                case MessageType.TEXT:
                    msg = JSR.Deserialize<PublishMessage>(msgStr);
                    break;
                case MessageType.GETONLINEUSERS:
                    msg = JSR.Deserialize<GetOnlineUsers>(msgStr);
                    break;
                default:
                    break;
            }

            if(MessageReceived != null)
                MessageReceived(this, new MessageEvent(msg,clientInfo));
        }

        public byte[] EncodePacket(IComMessage iComMessage)
        {
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
        NEWUSERONLINE = 3,
        TEXT = 4,
        GETONLINEUSERS = 2,
        USERLOGON = 1,
        NOMATCHINGTYPE = 0
    }
}
