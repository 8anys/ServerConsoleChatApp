using System;
using System.Collections.Generic;
using System.Text;

namespace serverChat
{
    public static class Controller
    {
        private static readonly int MaxMessages = 100;
        public static List<Message> ChatHistory { get; private set; } = new List<Message>();

     
        public struct Message
        {
            public string UserName { get; }
            public string Data { get; }

            public Message(string userName, string data)
            {
                UserName = userName;
                Data = data;
            }
        }


        public static void AddMessage(string userName, string messageContent)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(messageContent))
                return;

            if (ChatHistory.Count >= MaxMessages)
            {
                ClearChat();
            }

            var newMessage = new Message(userName, messageContent);
            ChatHistory.Add(newMessage);

            Console.WriteLine($"New message from {userName}.");
            ServerProgram.UpdateAllChats(); 
        }


        public static void ClearChat()
        {
            ChatHistory.Clear();
        }

  
        public static string GetChatAsString()
        {
            if (ChatHistory.Count == 0)
                return string.Empty;

            var chatStringBuilder = new StringBuilder("#updatechat&");
            foreach (var message in ChatHistory)
            {
                chatStringBuilder.AppendFormat("{0}~{1}|", message.UserName, message.Data);
            }

            return chatStringBuilder.ToString();
        }
    }
}
