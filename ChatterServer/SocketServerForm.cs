/*
 * Created by SharpDevelop.
 * User: Jayan Nair
 * Date: 02/01/2005
 * Time: 2:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using CommonClientServerLib;
using CommonClientServerLib.Messages;

namespace ChatterServer
{
	/// <summary>
	/// Description of SocketServer.	
	/// </summary>
	public partial class SocketServerForm : System.Windows.Forms.Form
	{
		public delegate void UpdateRichEditCallback(string msg, RichTextBox richTextBox);
        public delegate void UpdateClientListCallback();
        public delegate void SetTextboxMsgCallBack(string msg, TextBox box);

        private Server server;
		
		public SocketServerForm()
		{
			InitializeComponent();
			
			// Display the local IP address on the GUI
			textBoxIP.Text = NetHelper.GetIP();
		}

        void server_clientReceivedMessageEvent(object sender, CommonClientServerLib.Messages.MessageEvent message)
        {
            IComMessage msg = message.GetMessage;

            switch (msg.Id)
            {
                case MessageType.TEXT:
                    {
                        TextMessage textmsg = (TextMessage)msg;
                        AppendToRichEditControl(message.ClientID.Name + " wrote: " + textmsg.Text + Environment.NewLine,richTextBoxReceivedMsg);
                    }
                    break;
                case MessageType.USER:
                    {
                    UserLoggedOnMessage userMsg = (UserLoggedOnMessage)msg;
                    Add(message.ClientID, listBoxClientList);
                    SetTextboxMsg(message.ClientID.Name + " connected.", textBoxMsg);
                    }
                    break;
                case MessageType.NOMATCHINGTYPE:
                    break;
                default:
                    break;
            }
        }

        void server_newConnectionEvent(object sender, ConnectionChangedEventArgs args)
        {
            SetTextboxMsg("Client " + args.ClientInfo.ID + " is connecting.", textBoxMsg);
        }

        private void UpdateControls(bool listening)
        {
            buttonStartListen.Enabled = !listening;
            buttonStopListen.Enabled = listening;
        }

        public void Add(ClientInfo item, ListBox list)
        {
            if (list.InvokeRequired)
            {
                list.BeginInvoke(new MethodInvoker(delegate
                {
                    Add(item, list);
                }));
            }
            else
            {
                list.Items.Add(item);
            }
        }

        public void Remove(ClientInfo item, ListBox list)
        {
            if (list.InvokeRequired)
            {
                list.BeginInvoke(new MethodInvoker(delegate
                {
                    Remove(item, list);
                }));
            }
            else
            {
                list.Items.Remove(item);
            }
        }

        #region Buttons

        void ButtonStartListenClick(object sender, System.EventArgs e)
        {
            try
            {
                // Check the port value
                if (textBoxPort.Text == "")
                {
                    MessageBox.Show("Please enter a Port Number");
                    return;
                }
                string portStr = textBoxPort.Text;
                int port = System.Convert.ToInt32(portStr);

                // Create the server part
                server = new Server(port);
                server.newConnectionEvent += new Server.NewConnectionEvent(server_newConnectionEvent);
                server.clientReceivedMessageEvent += new Server.ClientReceivedMessageEvent(server_clientReceivedMessageEvent);
                server.clientDisconnectedEvent += new Server.ClientDisconnectedEvent(server_clientDisconnectedEvent);
                server.StartListning();

                UpdateControls(true);
            }
            catch (Exception se)
            {
                MessageBox.Show(se.Message);
            }

        }

        void server_clientDisconnectedEvent(object sender, ConnectionChangedEventArgs args)
        {
            Remove(args.ClientInfo, listBoxClientList);
            SetTextboxMsg(args.ClientInfo.Name + " disconnected", textBoxMsg);
        }

		private void ButtonSendMsgClick(object sender, System.EventArgs e)
		{
            if (listBoxClientList.SelectedIndex == -1)
            {
                MessageBox.Show("Der skal vælges en modtager","Send besked",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            ClientInfo client = (ClientInfo)listBoxClientList.Items[listBoxClientList.SelectedIndex];
            server.SendMsgToClient(richTextBoxSendMsg.Text, client.ID);
            richTextBoxSendMsg.Text = "";
		}
		
		private void ButtonStopListenClick(object sender, System.EventArgs e)
		{
			server.CloseSockets();			
			UpdateControls(false);
		}
	
		private void ButtonCloseClick(object sender, System.EventArgs e)
		{
            if(server != null)
			    server.CloseSockets();
			Close();
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			richTextBoxReceivedMsg.Clear();
		}

        private void btnBroadCast_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = richTextBoxSendMsg.Text;
                richTextBoxSendMsg.Text = "";
                msg = "Server Msg: " + msg;
                server.BroadcastMsg(msg);
            }
            catch (Exception se)
            {
                MessageBox.Show(se.Message);
            }
        }

        #endregion

        #region UpdateUI elements
        /// <summary>
        /// Method to update text on a textbox.
        /// </summary>
        /// <param name="msg">String type message</param>
        /// <param name="box">Textbox to set text</param>
        private void SetTextboxMsg(string msg, TextBox box)
        {
            if (box.InvokeRequired)
            {
                object[] pList = { msg, box };
                box.BeginInvoke(new SetTextboxMsgCallBack(SetTextboxMsg), pList);
            }
            else
            {
                box.Text = msg;
            }
        }
        /// <summary>
        /// Method to append text on a RichTextBox
        /// </summary>
        /// <param name="msg">String type message</param>
        /// <param name="richTextBox">Richtextbox to append text</param>
        private void AppendToRichEditControl(string msg, RichTextBox richTextBox)
        {
            // Check to see if this method is called from a thread 
            // other than the one created the control
            if (richTextBox.InvokeRequired)
            {
                // We cannot update the GUI on this thread.
                // All GUI controls are to be updated by the main (GUI) thread.
                // Hence we will use the invoke method on the control which will
                // be called when the Main thread is free
                // Do UI update on UI thread
                object[] pList = { msg, richTextBox };
                richTextBox.BeginInvoke(new UpdateRichEditCallback(AppendToRichEditControl), pList);
            }
            else
            {
                // This is the main thread which created this control, hence update it
                // directly 
                richTextBox.AppendText(msg);
            }
        }
        #endregion
    }
}
