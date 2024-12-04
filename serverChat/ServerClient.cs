using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace serverChat
{
    public class ServerClient
    {
        private  string _userName;
        private Socket _clientSocket;
        private  Thread _clientThread;

        public ServerClient(Socket clientSocket)
        {
            _clientSocket = clientSocket;
            _clientThread = new Thread(ListenForMessages) { IsBackground = true };
            _clientThread.Start();
        }

        public string UserName => _userName;

        private void ListenForMessages()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = _clientSocket.Receive(buffer);
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ProcessCommand(receivedData);
                }
                catch
                {
                    ServerProgram.EndClient(this);
                    break;
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                _clientSocket.Close();
                _clientThread.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Disconnect: {ex.Message}");
            }
        }

        private void ProcessCommand(string commandData)
        {
            if (commandData.StartsWith("#setname"))
            {
                _userName = commandData.Split('&')[1];
                RefreshChat();
            }
            else if (commandData.StartsWith("#newmsg"))
            {
                string messageContent = commandData.Split('&')[1];
                Controller.AddMessage(_userName, messageContent);
                ServerProgram.BroadcastMessage(messageContent, this);
            }
        }

        public void RefreshChat()
        {
            Send(Controller.GetChatAsString());
        }

        public void Send(string message)
        {
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                _clientSocket.Send(messageBytes);
                Console.WriteLine("Message sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Send: {ex.Message}");
                ServerProgram.EndClient(this);
            }
        }
    }
}
