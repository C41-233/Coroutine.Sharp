using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Coroutines;
using Coroutines.Await;

namespace BestAiServer
{
    public class Program
    {

        private static CoroutineManager.Container Container;


        public static void Main(string[] args)
        {
            var coroutineManager = new CoroutineManager();
            Container = coroutineManager.CreateContainer();

            Container.StartCoroutine(MainLoop);
            while (true)
            {
                coroutineManager.OneLoop();
                Thread.Sleep(10);
            }
        }

        private static async IWaitable MainLoop()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] server start...");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 8077));
            socket.Listen(5);
            while (true)
            {
                var client = await socket.AcceptAsync();
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] connect client {client.RemoteEndPoint}");
                ProcessClient(client);
            }
        }

        private static void ProcessClient(Socket client)
        {
            Container.StartCoroutine(Run, client).Catch(e =>
            {
                Console.Error.WriteLine(e);
                client.Close();
            });

            async IWaitable Run(Socket socket)
            {
                var bs = new byte[1024];
                var remote = socket.RemoteEndPoint;
                while (true)
                {
                    int nread;
                    try
                    {
                        nread = await socket.ReceiveAsync(new ArraySegment<byte>(bs), SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        if (ex is SocketException e)
                        {
                            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Close {remote} : {e.SocketErrorCode}");
                        }
                        else
                        {
                            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Error receive {remote} : {ex}");
                        }
                        socket.Close();
                        break;
                    }
                    if (nread <= 0)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Close {remote}");
                        socket.Close();
                        break;
                    }

                    var receive = Encoding.UTF8.GetString(bs, 0, nread);
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] receive {receive} from {socket.RemoteEndPoint}");

                    var answer = Answer(receive) + "\n";
                    int nsend;
                    try
                    {
                        var sendData = Encoding.UTF8.GetBytes(answer);
                        nsend = await socket.SendAsync(new ArraySegment<byte>(sendData), SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Error send {remote} : {ex}");
                        socket.Close();
                        break;
                    }

                    if (nsend <= 0)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Close {remote}");
                        socket.Close();
                        break;
                    }
                }
            }
        }

        private static string Answer(string question)
        {
            question = question.Trim();

            if (question.Contains("什么"))
            {
                return "不知道！";
            }

            question = question.Replace("你", "我");
            if (question.EndsWith("吗？"))
            {
                return question.Substring(0, question.Length - 2) + "！";
            }

            if (question.EndsWith("？"))
            {
                return question.Substring(0, question.Length - 1) + "！";
            }

            return question;
        }

    }
}
