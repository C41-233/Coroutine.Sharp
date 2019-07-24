using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BestAiClient
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 8000);
            var bs = new byte[1024];
            while (true)
            {
                var message = Console.ReadLine();
                socket.Send(Encoding.UTF8.GetBytes(message));

                var nread = socket.Receive(bs);
                if (nread <= 0)
                {
                    break;
                }

                Console.WriteLine(Encoding.UTF8.GetString(bs, 0, nread));
            }
        }

    }
}
