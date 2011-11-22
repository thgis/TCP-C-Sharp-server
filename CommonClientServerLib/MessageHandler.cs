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
        /// <summary>
        /// Event that is raised when a Message is received.
        /// </summary>
        public event MessageReceivedHandler MessageReceived;


        public MessageHandler()
        {
        }

        public void DecodePacketJson(List<byte> data, ClientInfo clientInfo)
        {
            IComMessage msg;
            JavaScriptSerializer JSR = new JavaScriptSerializer();

            msg = JSR.Deserialize<JsonUserLogOn>(Encoding.ASCII.GetString(data.ToArray()));

            if(MessageReceived != null)
                MessageReceived(this, new MessageEvent(msg,clientInfo));
        }

        public void DecodePacket(List<byte> temp, ClientInfo clientID)
        {
            MessageType type = FindMessageType(temp);
            IComMessage message = null;

            switch (type)
            {
                case MessageType.TEXT:
                    message = new TextMessage();
                    break;
                case MessageType.USER:
                    message = new UserLoggedOnMessage();
                    break;
                case MessageType.NOMATCHINGTYPE:
                    break;
                default:
                    break;
            }
            message.deserialize(temp);
            if(MessageReceived != null)
                MessageReceived(this,new MessageEvent(message,clientID));
        }

        private MessageType FindMessageType(List<byte> temp)
        {
            MessageType messageType = MessageType.NOMATCHINGTYPE;

            string[] msgs = Encoding.UTF8.GetString(temp.ToArray()).Split(':');
            if (msgs[0] == "User")
                messageType = MessageType.USER;
            else
                messageType = MessageType.TEXT;
            return messageType;
        }
    }

    public enum MessageType
    {
        TEXT,
        USER,
        NOMATCHINGTYPE
    }
}
