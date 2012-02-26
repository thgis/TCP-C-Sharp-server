using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonClientServerLib.Messages;
using MySql.Data.MySqlClient;
using System.IO;

namespace CommonClientServerLib
{
    public class MySQLDBHandler
    {
        public static MySqlConnection GetConnection()
        {
            //string MyConString = "SERVER=176.34.177.147:3306;" +
            //    "DATABASE=ChatterDB;" +
            //    "UID=ChatterServer;" +
            //    "PASSWORD=BigBangTheory;";

            string MyConString = "SERVER=127.0.0.1;" +
                                 "DATABASE=ChatterDB;" +
                                 "UID=ChatterServer;" +
                                 "PASSWORD=BigBangTheory;";

                MySqlConnection connection = new MySqlConnection(MyConString);

                connection.Open();

                return connection;
        }

        public static List<PublishMessage> GetStoredMessages(long timestamp)
        {
            MySqlConnection connection = GetConnection();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader Reader;

            command.CommandText = "select * from publishmessages";
            Reader = command.ExecuteReader();

            List<PublishMessage> sendMessageList = new List<PublishMessage>();
            while (Reader.Read())
            {
                PublishMessage msg = new PublishMessage();

                msg.sender = Reader.GetString("sender");
                msg.receiver = Reader.GetString("receiver");
                msg.message = Reader.GetString("message");
                msg.timeStamp = Reader.GetInt64("timeStamp");

                sendMessageList.Add(msg);
            }
            connection.Close();

            return sendMessageList;
        }

        public static int SavePublisMsg(PublishMessage msg)
        {
            MySqlConnection connection = GetConnection();
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = @"INSERT INTO publishmessages (sender, receiver, message, timeStamp) VALUES(@sender,@receiver,@message,@timeStamp);";

            command.Parameters.AddWithValue("@sender", msg.sender);
            command.Parameters.AddWithValue("@receiver", msg.receiver);
            command.Parameters.AddWithValue("@message", msg.message);
            command.Parameters.AddWithValue("@timeStamp", msg.timeStamp);

            return command.ExecuteNonQuery();
        }
    }
}
