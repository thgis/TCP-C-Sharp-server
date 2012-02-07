using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Web.Script.Serialization;
using CommonClientServerLib.Messages;
using CommonClientServerLib;

namespace ChatterClient
{
    public partial class formChatter : Form
    {
        private Client client;
        public delegate void UpdateRichEditCallback(string text);
        public delegate void UpdateControlsCallBack(bool connected);

        public formChatter()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new Client();
            client.MessageReceived += new MessageReceived(client_MessageReceived);
            textBoxIP.Text = client.GetIP();
        }

        void client_MessageReceived(object sender, MessageEvent message)
        {
            switch (message.GetMessage.type)
            {
                case MessageType.SENDMESSAGE:
                    break;
                case MessageType.PUBLISHMESSAGE:
                    {
                        string text;
                        PublishMessage msg = (PublishMessage)message.GetMessage;

                        if (msg.sender == client.UserName)
                            text = "You say: " + msg.message;
                        else
                            text = msg.sender + " says: " + msg.message;

                        AppendToRichEditControl(text);
                    }
                    break;
                case MessageType.NEWUSERONLINE:
                    {
                        NewUserOnline msg = (NewUserOnline)message.GetMessage;

                        AppendToRichEditControl(msg.userName + " has logged on:-)");
                    }
                    break;
                case MessageType.GETONLINEUSERS:
                    break;
                case MessageType.USERLOGON:
                    {
                        UserLogOn msg = (UserLogOn)message.GetMessage;

                        AppendToRichEditControl("Welcome " + txtUserName.Text + "! You got the ID " + msg.id);
                    }
                    break;
                case MessageType.NOMATCHINGTYPE:
                    break;
                default:
                    break;
            }
        }

        void ButtonCloseClick(object sender, System.EventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }
            Close();
        }

        void ButtonConnectClick(object sender, System.EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUserName.Text))
            {
                MessageBox.Show("Navn skal udfyldes før tilslutning!", "Brugernavn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // See if we have text on the IP and Port text fields
            if (textBoxIP.Text == "" || textBoxPort.Text == "")
            {
                MessageBox.Show("IP Address and Port Number are required to connect to the Server\n");
                return;
            }
            try
            {
                bool result = client.connect(textBoxIP.Text, textBoxPort.Text, txtUserName.Text); 
                UpdateControls(result);
            }
            catch (SocketException se)
            {
                string str;
                str = "\nConnection failed, is the server running?\n" + se.Message;
                MessageBox.Show(str);
                UpdateControls(false);
            }
        }

        void ButtonSendMessageClick(object sender, System.EventArgs e)
        {
            SendTextMsg();
        }

        private void SendTextMsg()
        {
            string msg = richTextTxMessage.Text;
            richTextTxMessage.Text = "";

            client.sendmessage(msg);
        }

        // This method could be called by either the main thread or any of the
        // worker threads
        private void AppendToRichEditControl(string msg)
        {
            // Check to see if this method is called from a thread 
            // other than the one created the control
            if (richTextRxMessage.InvokeRequired)
            {
                // We cannot update the GUI on this thread.
                // All GUI controls are to be updated by the main (GUI) thread.
                // Hence we will use the invoke method on the control which will
                // be called when the Main thread is free
                // Do UI update on UI thread
                object[] pList = { msg };
                richTextRxMessage.BeginInvoke(new UpdateRichEditCallback(OnUpdateRichEdit), pList);
            }
            else
            {
                // This is the main thread which created this control, hence update it
                // directly 
                OnUpdateRichEdit(msg);
            }
        }
        // This UpdateRichEdit will be run back on the UI thread
        // (using System.EventHandler signature
        // so we don't need to define a new
        // delegate type here)
        private void OnUpdateRichEdit(string msg)
        {
            richTextRxMessage.AppendText(msg + Environment.NewLine);
        }

        private void UpdateControls(bool connected)
        {
            buttonConnect.Enabled = !connected;
            buttonDisconnect.Enabled = connected;
            string connectStatus = connected ? "Connected" : "Not Connected";
            textBoxConnectStatus.Text = connectStatus;
        }
        void ButtonDisconnectClick(object sender, System.EventArgs e)
        {
            if (client != null)
            {
                client.Close();
                UpdateControls(false);
            }
        }
        

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            richTextRxMessage.Clear();
        }

        private void richTextTxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                SendTextMsg();
                e.Handled = true;
            }
        }		
    }
}
