using System;
using System.Net.Sockets;

namespace Coroutines
{
    internal class WaitForAccept : WaitableTask<Socket>
    {

        public WaitForAccept(Socket socket, Socket acceptSocket, int receiveSize)
        {
            try
            {
                socket.BeginAccept(acceptSocket, receiveSize, AcceptCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        public WaitForAccept(Socket socket, int receiveSize)
        {
            try
            {
                socket.BeginAccept(receiveSize, AcceptCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        public WaitForAccept(Socket socket)
        {
            try
            {
                socket.BeginAccept(AcceptCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            try
            {
                var acceptSocket = socket.EndAccept(ar);
                Success(acceptSocket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

    }

}
