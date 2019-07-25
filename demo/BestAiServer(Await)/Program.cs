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
            Console.WriteLine("server start...");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 8077));
            socket.Listen(5);
            while (true)
            {
                var client = await WaitFor.Accept(socket);
                Console.WriteLine($"connect client {client.RemoteEndPoint}");
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
                        nread = await WaitFor.Receive(socket, bs);
                    }
                    catch (Exception ex)
                    {
                        if (ex is SocketException e)
                        {
                            Console.WriteLine($"Close {remote} : {e.SocketErrorCode}");
                        }
                        else
                        {
                            Console.WriteLine($"Error receive {remote} : {ex}");
                        }
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

                    var answer = Answer(receive) + "\n";
                    int nsend;
                    try
                    {
                        nsend = await WaitFor.Send(socket, Encoding.UTF8.GetBytes(answer));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error send {remote} : {ex}");
                        socket.Close();
                        break;
                    }

                    if (nsend <= 0)
                    {
                        Console.WriteLine($"Close {remote}");
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
