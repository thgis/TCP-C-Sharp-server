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

            if(test.ContainsKey("Id"))
                msgType = (MessageType)int.Parse(test["Id"].ToString());

            switch (msgType)
            {
                case MessageType.USER:
                    msg = JSR.Deserialize<UserLogOn>(msgStr);
                    break;
                case MessageType.TEXT:
                    break;
                case MessageType.GETONLINEPEOPLE:
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

            return Encoding.ASCII.GetBytes(msg);
        }
    }

    public enum MessageType
    {
        TEXT = 3,
        GETONLINEPEOPLE = 2,
        USER = 1,
        NOMATCHINGTYPE = 0
    }
}
