using System;
using System.Net;
using System.Net.Sockets;

namespace Coroutines
{
    internal class WaitForConnect : WaitableTask
    {

        public WaitForConnect(Socket socket, string host, int port)
        {
            try
            {
                socket.BeginConnect(host, port, ConnectCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        public WaitForConnect(Socket socket, IPAddress[] addresses, int port)
        {
            try
            {
                socket.BeginConnect(addresses, port, ConnectCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        public WaitForConnect(Socket socket, IPAddress address, int port)
        {
            try
            {
                socket.BeginConnect(address, port, ConnectCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        public WaitForConnect(Socket socket, EndPoint endPointt)
        {
            try
            {
                socket.BeginConnect(endPointt, ConnectCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            try
            {
                socket.EndConnect(ar);
                Success();
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

    }

}
