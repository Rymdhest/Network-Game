using Network_Game;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpServer
{
    class Program
    {
        private static Socket _serverSocket;
        private static readonly List<Socket> ClientSockets = new List<Socket>();
        private const int BufferSize = 2048;
        private const int Port = 1337;
        private static readonly byte[] Buffer = new byte[BufferSize];
        private static bool _closing;
        private static List<Game> games = new List<Game>();

        static void Main()
        {
            Console.Title = "Server";
            SetupServer();

            //Vänta här!
            Console.ReadLine();
            _closing = true;
            CloseAllSockets();
            Thread.Sleep(2000);
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }

        private static void CloseAllSockets()
        {
            foreach (Socket socket in ClientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            _serverSocket.Close();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            if (_closing)
                return;

            Socket socket = _serverSocket.EndAccept(ar);
            ClientSockets.Add(socket);
            socket.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            games.Add(new Game());
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            if (_closing)
                return;

            Socket current = (Socket)ar.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(ar);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                current.Close();
                ClientSockets.Remove(current);
                return;
            }

            string text = Encoding.UTF8.GetString(Buffer, 0, received);
            Console.WriteLine("Received Text: " + text);

            if (Int32.TryParse(text, out int number))
            {
                if (number >= 0 && number  <= 8)
                {
                    games[0].ResolveInput(number);
                    current.Send(Encoding.UTF8.GetBytes(games[0].getBoardAsString()));
                }
            } else if (text.ToLower() =="exit")
            {
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                ClientSockets.Remove(current);
                Console.WriteLine("Client disconnected");
                return;
            }
            else if (text.ToLower() == "restart")
            {
                games[0].restart();
                current.Send(Encoding.UTF8.GetBytes(games[0].getBoardAsString()));
            }
            else
            {
                Console.WriteLine(text+" is an invalid request");
                current.Send(Encoding.UTF8.GetBytes("Invalid request"));
                Console.WriteLine("Warning Sent");
            }


            current.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, current);
        }
    }
}