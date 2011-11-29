using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using CommonClientServerLib;
using CommonClientServerLib.Messages;

namespace ChatterServer
{
    public delegate void DisconnectedEvent(object sender, EventArgs args); 

    public class Client : IDisposable
    {
        private AsyncCallback pfnWorkerCallBack;
        private PacketHandler packetHandler;
        private MessageHandler messageHandler;
        private ClientInfo clientInfo;

        public event DisconnectedEvent DisconnectedEvent;

        public Client(Socket socket, int id)
        {
            dataBuffer = new byte[1024];
            Socket = socket;
            packetHandler = new PacketHandler();
            packetHandler.CompletePacketReceived += new PacketHandler.CompletePacketReceivedEventHandler(packetHandler_CompletePacketReceived);

            messageHandler = new MessageHandler();

            clientInfo = new ClientInfo();
            clientInfo.ID = id;

            // Let the worker Socket do the further processing for the 
            // just connected client
            WaitForData();
        }

        void packetHandler_CompletePacketReceived(object sender, CompletePacketReceivedArgs args)
        {
            //messageHandler.DecodePacket(args.Data, clientInfo);
            messageHandler.DecodePacketJson(args.Data, clientInfo);
        }

        public ClientInfo ClientInfo
        {
            get
            {
                return clientInfo;
            }
        }

        private Socket Socket { get; set; }
        private byte[] dataBuffer { get; set; }

        public MessageHandler GetMessageHandler
        {
            get
            {
                return messageHandler;
            }
        }

        public void SendMessage(IComMessage msg)
        {
            byte[] byData = messageHandler.EncodePacket(msg);

            Socket.Send(byData);
        }

        // This the call back function which will be invoked when the socket
        // detects any client writing of data on the stream
        private void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                // Complete the BeginReceive() asynchronous call by EndReceive() method
                // which will return the number of characters written to the stream 
                // by the client
                int iRx = Socket.EndReceive(asyn);

                if (!SocketConnected(Socket))
                    throw new SocketException((int)SocketError.ConnectionReset);

                for (int i = 0; i < iRx; i++)
                {
                    packetHandler.DetectPacket(dataBuffer[i]);
                }
                // Continue the waiting for data on the Socket
                WaitForData();

            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                {
                    // Make event that connection closed unexpectedly.
                    if (DisconnectedEvent != null)
                        DisconnectedEvent(this, new EventArgs());
                }
                else
                {
                    // Some error event
                }
            }
        }
        // Start waiting for data from the client
        private void WaitForData()
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    // Specify the call back function which is to be 
                    // invoked when there is any write activity by the 
                    // connected client
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }
                Array.Clear(dataBuffer, 0, dataBuffer.Length);

                Socket.BeginReceive(dataBuffer, 0,
                    dataBuffer.Length,
                    SocketFlags.None,
                    pfnWorkerCallBack,
                    null);
            }
            catch (SocketException se)
            {
                //MessageBox.Show(se.Message);
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

        #region IDisposable Members

        public void Dispose()
        {
            Socket.Disconnect(false);
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            Socket = null;
        }
        #endregion
    }

}
