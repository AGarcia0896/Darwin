using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace Pyserver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Port: ");
            String _port = Console.ReadLine();
            byte[] buffer = new Byte[1024];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(_port));
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket socket = listener.Accept();
                    String data = null;

                    while (true)
                    {
                        int bytesRec = socket.Receive(buffer);
                        data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Text received : {0}", data);
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    socket.Send(msg);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }
    }
}
