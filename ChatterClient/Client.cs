using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using CommonClientServerLib.Messages;
using CommonClientServerLib;
using System.Net.Sockets;

namespace ChatterClient
{
    public delegate void DisconnectedEvent(object sender, EventArgs args);
    public delegate void MessageReceived(object sender, MessageEvent message);

    class Client
    {
        private PacketHandler packetHandler;
        private byte[] dataBuffer = new byte[1024];
        private AsyncCallback m_pfnCallBack;
        private Socket m_clientSocket;

        public event DisconnectedEvent DisconnectedEvent;
        public event MessageReceived MessageReceived;

        public Client()
        {
            packetHandler = new PacketHandler();
            packetHandler.CompletePacketReceived += new PacketHandler.CompletePacketReceivedEventHandler(packetHandler_CompletePacketReceived);
        }

        public string UserName { get; set; }

        void packetHandler_CompletePacketReceived(object sender, CompletePacketReceivedArgs args)
        {

            IComMessage msg = MessageHandler.DecodePacketJson(args.Data);

            switch (msg.type)
            {
                case MessageType.SENDMESSAGE:
                    break;
                case MessageType.PUBLISHMESSAGE:
                    if (MessageReceived != null)
                    {
                        MessageReceived(this,new MessageEvent(msg,null));
                    }
                    break;
                case MessageType.NEWUSERONLINE:
                    break;
                case MessageType.GETONLINEUSERS:
                    break;
                case MessageType.USERLOGON:
                    if (MessageReceived != null)
                    {
                        MessageReceived(this,new MessageEvent(msg,null));
                    }
                    break;
                case MessageType.NOMATCHINGTYPE:
                    break;
                default:
                    break;
            }
        }

        internal void Close()
        {
            m_clientSocket.Close();
            m_clientSocket = null;
        }

        public bool connect(string Ip, string IPortNo, string UserName)
        {
            this.UserName = UserName;
            // Create the socket instance
            m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Cet the remote IP address
            IPAddress ip = IPAddress.Parse(Ip);
            int iPortNo = System.Convert.ToInt16(IPortNo);
            // Create the end point 
            IPEndPoint ipEnd = new IPEndPoint(ip, iPortNo);
            // Connect to the remote host
            m_clientSocket.Connect(ipEnd);
            if (m_clientSocket.Connected)
            {
                UserLogOn userlogon = new UserLogOn();
                userlogon.userName = this.UserName;
                SendIMessage(userlogon);
                //Wait for data asynchronously 
                WaitForData();

                return true;
            }
            return false;
        }

        public void sendmessage(string msg)
        {
            SendMessage SMessage = new SendMessage();
            SMessage.message = msg;
            SMessage.sender = UserName;
            SendIMessage(SMessage);
        }

        private void SendIMessage(IComMessage SMessage)
        {
            byte[] data = MessageHandler.EncodePacket(SMessage);
            m_clientSocket.Send(data);
        }
        public void WaitForData()
        {
            try
            {
                if (m_pfnCallBack == null)
                {
                    m_pfnCallBack = new AsyncCallback(OnDataReceived);
                }
                Array.Clear(dataBuffer, 0, dataBuffer.Length);

                // Start listening to the data asynchronously
                m_clientSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, m_pfnCallBack,null);
            }
            catch (SocketException se)
            {
                //MessageBox.Show(se.Message);
            }

        }
        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                int iRx = m_clientSocket.EndReceive(asyn);
                
                if (!SocketConnected(m_clientSocket))
                    throw new SocketException((int)SocketError.ConnectionReset);

                for (int i = 0; i < iRx; i++)
                {
                    packetHandler.DetectPacket(dataBuffer[i]);
                }

                WaitForData();    
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.ConnectionReset)
                {
                    if (DisconnectedEvent != null)
                        DisconnectedEvent(this, new EventArgs());
                }
                else
                { 
                    // show error to gui
                }
            }
        }

        bool SocketConnected(Socket socket)
        {
            bool part1 = socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            if (part1 & part2)
                return false;
            else
                return true;
        }

        //----------------------------------------------------	
        // This is a helper function used (for convenience) to 
        // get the IP address of the local machine
        //----------------------------------------------------
        public String GetIP()
        {
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            // Grab the first IP addresses
            String IPStr = "";
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPStr = ipaddress.ToString();
                    return IPStr;
                }
            }
            return IPStr;
        }
    }
}
