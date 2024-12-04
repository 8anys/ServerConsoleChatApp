using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace serverChat
{
    class Program
    {
        private static readonly string ServerHost = "localhost";
        private static readonly int ServerPort = 9933;
        private static Thread _serverThread;

        static void Main(string[] args)
        {
            _serverThread = new Thread(StartServer) { IsBackground = true };
            _serverThread.Start();

            while (true)
            {
                HandleAdminCommands(Console.ReadLine());
            }
        }

        private static void HandleAdminCommands(string command)
        {
            command = command.ToLower();

            if (command.Contains("/getusers"))
            {
                int userCount = ServerProgram.Clients.Count;
                if (userCount == 0)
                {
                    Console.WriteLine("No users connected.");
                }
                else
                {
                    for (int i = 0; i < userCount; i++)
                    {
                        Console.WriteLine($"[{i}]: {ServerProgram.Clients[i].UserName}");
                    }
                }
            }
        }

        private static void StartServer()
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(ServerHost);
                IPAddress ipAddress = ipHost.AddressList[0];
                IPEndPoint endPoint = new IPEndPoint(ipAddress, ServerPort);

                Socket serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(1000);

                Console.WriteLine($"Server has been started on IP: {endPoint}.");

                while (true)
                {
                    try
                    {
                        Socket clientSocket = serverSocket.Accept();
                        ServerProgram.NewClient(clientSocket);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server startup error: {ex.Message}");
            }
        }
    }
}
