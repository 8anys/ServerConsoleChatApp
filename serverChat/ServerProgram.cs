using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace serverChat
{
    public static class ServerProgram
    {
        public static List<ServerClient> Clients { get; private set; } = new List<ServerClient>();

        public static void NewClient(Socket clientSocket)
        {
            try
            {
                var newClient = new ServerClient(clientSocket);
                Clients.Add(newClient);
                Console.WriteLine($"New client connected: {clientSocket.RemoteEndPoint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with NewClient: {ex.Message}");
            }
        }

        public static void EndClient(ServerClient client)
        {
            try
            {
                client.Disconnect();
                Clients.Remove(client);
                Console.WriteLine($"User {client.UserName} has been disconnected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with EndClient: {ex.Message}");
            }
        }

        public static void BroadcastMessage(string message, ServerClient sender)
        {
            try
            {
                foreach (var client in Clients)
                {
                    if (client != sender)
                    {
                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with BroadcastMessage: {ex.Message}");
            }
        }

        public static void UpdateAllChats()
        {
            try
            {
                foreach (var client in Clients)
                {
                    client.RefreshChat();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with UpdateAllChats: {ex.Message}");
            }
        }
    }
}