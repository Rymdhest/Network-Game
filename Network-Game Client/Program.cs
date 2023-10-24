using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpClient
{
    class Program
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int Port = 1337;
        private const int BufferSize = 2048;
        static void Main()
        {
            Console.Title = "Client";
            ConnectToServer();
            RequestLoop();

            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
        }

        private static void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    ClientSocket.Connect(IPAddress.Loopback, Port);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }
            Console.Clear();
            Console.WriteLine("Connected");
        }

        private static void RequestLoop()
        {
            Console.WriteLine(@"<Type ""exit"" to properly disconnect client>");
            string requestSent = string.Empty;

            try
            {
                while (requestSent.ToLower() != "exit")
                {
                    Console.Write("Send a request: ");
                    requestSent = Console.ReadLine();
                    ClientSocket.Send(Encoding.UTF8.GetBytes(requestSent), SocketFlags.None);
                    ReceiveResponse();
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error! - Lost server.");
                Console.ReadLine();
            }

        }

        private static void ReceiveResponse()
        {
            var buffer = new byte[BufferSize];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0)
                return;
            PrintBoard(Encoding.UTF8.GetString(buffer, 0, received));
        }
        private static void PrintBoard(string serverState)
        {
            string board = "";
            for (int y = 0; y<3; y++)
            {

                for (int x = 0; x < 3; x++)
                {
                    board += "|";
                    int state = Int32.Parse(""+serverState[y * 3 + x]);
                    if (state == 0)
                    {
                        board += " ";
                    } else if (state == 1)
                    {
                        board += "X";
                    } else if (state == 2)
                    {
                        board += "O";
                    }
                    else
                    {
                        board += "E";
                    }
                    board += "|";
                }
                board += "\n";
            }

            Console.WriteLine(board);
        }
    }
}