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
                client.GetMessageHandler.MessageReceived += new CommonClientServerLib.MessageReceivedHandler(GetMessageHandler_MessageReceived);
				
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

        void client_DisconnectedEvent(object sender, EventArgs args)
        {
            if (clientDisconnectedEvent != null)
                clientDisconnectedEvent(this, new ConnectionChangedEventArgs() { ClientInfo = ((Client)sender).ClientInfo });
            m_workerSocketList.Remove(sender);
        }

        void GetMessageHandler_MessageReceived(object sender, CommonClientServerLib.Messages.MessageEvent message)
        {
            if (message.GetMessage.Id == MessageType.USER)
            {
                message.ClientID.Name = ((UserLogOn)message.GetMessage).UserName;
            }

            if (clientReceivedMessageEvent != null)
                clientReceivedMessageEvent(this, message);
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
            foreach (Client item in m_workerSocketList)
            {
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
