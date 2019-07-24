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
                CoroutineManager.StartCoroutine(ProcessClient(client)).Catch(e =>
                {
                    Console.Error.WriteLine(e);
                });
            }
        }

        private static IEnumerable ProcessClient(Socket socket)
        {
            var bs = new byte[1024];
            var remote = socket.RemoteEndPoint;
            while (true)
            {
                yield return WaitFor.Receive(socket, bs).With(out var nread);
                if (nread.IsError())
                {
                    Console.WriteLine($"Error {remote} : {nread.Exception}");
                    socket.Close();
                    break;
                }

                if (nread <= 0)
                {
                    Console.WriteLine($"Close {remote}");
                    socket.Close();
                    break;
                }

                var receive = Encoding.UTF8.GetString(bs, 0, nread);
                Console.WriteLine($"receive {receive} from {socket.RemoteEndPoint}");
            }
        }

    }
}
