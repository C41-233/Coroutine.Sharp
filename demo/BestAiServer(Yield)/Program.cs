﻿using System;
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

        private static CoroutineManager.Container Container;


        public static void Main(string[] args)
        {
            var coroutineManager = new CoroutineManager();
            Container = coroutineManager.CreateContainer();

            Container.StartCoroutine(MainLoop());
            while (true)
            {
                coroutineManager.OneLoop();
                Thread.Sleep(10);
            }
        }

        private static IEnumerable MainLoop()
        {
            Console.WriteLine("server start...");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 8077));
            socket.Listen(5);
            while (true)
            {
                yield return WaitFor.Accept(socket).With(out var client);
                Console.WriteLine($"connect client {client.R.RemoteEndPoint}");
                Container.StartCoroutine(ProcessClient(client)).Catch(e =>
                {
                    Console.Error.WriteLine(e);
                    client.R.Close();
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
                    if (nread.Exception is SocketException e)
                    {
                        Console.WriteLine($"Close {remote} : {e.SocketErrorCode}");
                    }
                    else
                    {
                        Console.WriteLine($"Error receive {remote} : {nread.Exception}");
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
                yield return WaitFor.Send(socket, Encoding.UTF8.GetBytes(answer)).With(out var nsend);
                if (nsend.IsError())
                {
                    Console.WriteLine($"Error send {remote} : {nread.Exception}");
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
