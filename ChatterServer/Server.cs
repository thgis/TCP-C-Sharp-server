using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using CommonClientServerLib;
using CommonClientServerLib.Messages;

namespace ChatterServer
{
    public class Server
    {
        // Delegates
        public delegate void NewConnectionEvent(object sender, ConnectionChangedEventArgs args);
        public delegate void ClientReceivedMessageEvent(object sender, MessageEvent message);
        public delegate void ClientDisconnectedEvent(object sender, ConnectionChangedEventArgs args); 

        // Events
        public event NewConnectionEvent newConnectionEvent;
        public event ClientReceivedMessageEvent clientReceivedMessageEvent;
        public event ClientDisconnectedEvent clientDisconnectedEvent;

        // An ArrayList is used to keep track of worker sockets that are designed
        // to communicate with each connected client. Make it a synchronized ArrayList
        // For thread safety
        private System.Collections.ArrayList m_workerSocketList = ArrayList.Synchronized(new System.Collections.ArrayList());
        private Socket m_mainSocket;
        private IPEndPoint ipLocal;

        public Server(int port)
        {
            m_mainSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
            ipLocal = new IPEndPoint(IPAddress.Any, port);
        }

        public void StartListning()
        {
            // Bind to local IP Address...
            m_mainSocket.Bind(ipLocal);
            // Start listening...
            m_mainSocket.Listen(4);
            // Create the call back for any client connections...
            m_mainSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);
        }

        // This is the call back function, which will be invoked when a client is connected
		public void OnClientConnect(IAsyncResult asyn)
		{
			try
			{
				// Here we complete/end the BeginAccept() asynchronous call
				// by calling EndAccept() - which returns the reference to
				// a new Socket object
				Socket workerSocket = m_mainSocket.EndAccept (asyn);

				// Now get next free ID
                int id = GetNextID();

                // Create a new client with suplied id and socket
                Client client = new Client(workerSocket, id);
                client.DisconnectedEvent += new DisconnectedEvent(client_DisconnectedEvent);
                client.MessageReceived += new MessageReceived(client_MessageReceived);
				
				// Add the workerSocket reference to our ArrayList
				m_workerSocketList.Add(client);

                if (newConnectionEvent != null)
                    newConnectionEvent(this, new ConnectionChangedEventArgs() { ClientInfo = client.ClientInfo });
							
				// Since the main Socket is now free, it can go back and wait for
				// other clients who are attempting to connect
				m_mainSocket.BeginAccept(new AsyncCallback ( OnClientConnect ),null);
			}
			catch(ObjectDisposedException)
			{
				System.Diagnostics.Debugger.Log(0,"1","\n OnClientConnection: Socket has been closed\n");
			}
			catch(SocketException se)
			{
				//MessageBox.Show ( se.Message );
                // let out a event
			}	
		}

        void client_MessageReceived(object sender, MessageEvent message)
        {
            IComMessage msg = message.GetMessage;
            switch (message.GetMessage.type)
            {
                case MessageType.SENDMESSAGE:
                    {
                        SendMessage sendMessage = (SendMessage)msg;
                        PublishMessage pbMsg = new PublishMessage();
                        pbMsg.message = sendMessage.message;
                        pbMsg.sender = sendMessage.sender;
                        pbMsg.timeStamp = GetTimeStamp();

                        BroadcastMsg(pbMsg);
                    }
                    break;
                case MessageType.GETONLINEUSERS:
                    {
                        GetOnlineUsers gou = (GetOnlineUsers)msg;
                        gou.userList = new List<string>();
                        foreach (Client item in m_workerSocketList)
                        {
                            gou.userList.Add(item.ClientInfo.Name);
                        }
                        SendMsgToClient(gou, message.ClientID);
                    }
                    break;
                case MessageType.USERLOGON:
                    {
                        message.ClientID.Name = ((UserLogOn)message.GetMessage).userName;

                        UserLogOn returnMsg = (UserLogOn)message.GetMessage;
                        returnMsg.id = message.ClientID.ID;
                        returnMsg.success = true;
                        returnMsg.errorMessage = "";

                        SendMsgToClient(returnMsg, message.ClientID);

                        NewUserOnline nuo = new NewUserOnline();
                        nuo.userName = message.ClientID.Name;

                        BroadCastExceptSender(nuo,message.ClientID);
                    }
                    break;
                case MessageType.GETNEWMESSAGES:
                    {
                        PublishMessage pm = new PublishMessage();
                        pm.sender = "Server";
                        pm.message = "Logging of messages is not implented";
                        pm.timeStamp = GetTimeStamp();

                        SendMsgToClient(pm, message.ClientID);
                    }
                    break;
                case MessageType.NOMATCHINGTYPE:
                    break;
                default:
                    break;
            }

            if (clientReceivedMessageEvent != null)
                clientReceivedMessageEvent(this, message);
        }

        private long GetTimeStamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        void client_DisconnectedEvent(object sender, EventArgs args)
        {
            if (clientDisconnectedEvent != null)
                clientDisconnectedEvent(this, new ConnectionChangedEventArgs() { ClientInfo = ((Client)sender).ClientInfo });
            m_workerSocketList.Remove(sender);
        }

        private int GetNextID()
        {
            int id = 0;
            bool inUse = true;

            if (m_workerSocketList.Count > 0)
            {
                while (id == 0 && inUse)
                {
                    while (inUse)
                    {
                        inUse = IsIDUsed(id);
                        if (inUse)
                            id++;
                    }
                }
            }
            else
                id = 0;
            return id;
        }

        public void SendMsgToClient(IComMessage msg, ClientInfo Client)
        {
            foreach (Client item in m_workerSocketList)
            {
                if (item.ClientInfo.ID == Client.ID)
                    item.SendMessage(msg);
            }
        }

        public void BroadcastMsg(IComMessage msg)
        {
            foreach (Client item in m_workerSocketList.ToArray())
            {
                item.SendMessage(msg);
            }
        }

        public void BroadCastExceptSender(IComMessage msg, ClientInfo client)
        {
            foreach (Client item in m_workerSocketList.ToArray())
            {
                if(item.ClientInfo.ID != client.ID)
                    item.SendMessage(msg);
            }
        }

        private bool IsIDUsed(int id)
        {
            foreach (Client item in m_workerSocketList)
            {
                if (item.ClientInfo.ID == id)
                {
                    return true;
                }
            }
            return false;
        }

        public void CloseSockets()
        {
            if (m_mainSocket != null)
            {
                m_mainSocket.Close();
            }
            Client client = null;
            for (int i = 0; i < m_workerSocketList.Count; i++)
            {
                client = (Client)m_workerSocketList[i];
                if (client != null)
                {
                    client = null;
                    m_workerSocketList.Remove(m_workerSocketList[i]);
                }
            }
        }
    }
}
