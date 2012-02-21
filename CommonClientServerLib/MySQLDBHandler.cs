using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonClientServerLib.Messages;
using MySql.Data.MySqlClient;

namespace CommonClientServerLib
{
    public class MySQLDBHandler
    {
        public static List<SendMessage> GetSendMessages(long timestamp)
        {
            string MyConString = "SERVER=localhost;" +
                "DATABASE=mydatabase;" +
                "UID=testuser;" +
                "PASSWORD=testpassword;";
            List<SendMessage> sendMessageList = new List<SendMessage>();
            MySqlConnection connection = new MySqlConnection(MyConString);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = "select * from sendMessages";
            connection.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                string thisrow = "";
                for (int i = 0; i < Reader.FieldCount; i++)
                    thisrow += Reader.GetValue(i).ToString() + ",";
            }
            connection.Close();

            return sendMessageList;
        }
    }
}
