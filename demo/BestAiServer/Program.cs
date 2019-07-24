using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Coroutines;

namespace BestAiServer
{
    public class Program
    {

        private static readonly CoroutineManager CoroutineManager = new CoroutineManager();


        public static void Main(string[] args)
        {
            CoroutineManager.StartCoroutine(MainLoop());
            while (true)
            {
                CoroutineManager.OneLoop();
                Thread.Sleep(10);
            }
        }

        private static IEnumerable MainLoop()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 8000));
            socket.Listen(5);
            while (true)
            {
                yield return WaitFor.Accept(socket).With(out var client);
                CoroutineManager.StartCoroutine(ProcessClient(client));
            }
        }

        private static IEnumerable ProcessClient(Socket socket)
        {
            var bs = new byte[1024];
            while (true)
            {
                yield return WaitFor.Receive(socket, bs).With(out var nread);
                if (!nread.IsSuccess() || nread <= 0)
                {
                    socket.Close();
                    break;
                }
                var receive = Encoding.UTF8.GetString(bs, 0, nread);
                Console.WriteLine($"receive {receive} from {socket.RemoteEndPoint}");
            }
        }

    }
}
