using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace SocketServer
{
    public class Client : IDisposable
    {
        public Client(Socket socket, int id)
        {
            dataBuffer = new byte[1024];
            Socket = socket;
            ID = id;
        }

        public Socket Socket { get; private set; }
        public int ID { get; private set; }
        public string Name { get; set; }
        public byte[] dataBuffer { get; private set; }

        public void SendMessage(string msg)
        {
            // Convert the reply to byte array
            byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg);

            Socket.Send(byData);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Socket.Disconnect(false);
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            Socket = null;
        }

        public override string ToString()
        {
            return "Client: " + ID + " brugernavn: " + Name;
        }

        #endregion
    }


}
