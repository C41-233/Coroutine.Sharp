using System;
using System.Net;
using System.Net.Sockets;

namespace Coroutine.Wait
{
    internal class WaitForSendTo : WaitableTask<int>
    {

        public WaitForSendTo(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags, EndPoint endPoint)
        {
            try
            {
                socket.BeginSendTo(buffer, offset, size, flags, endPoint, SendToCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        private void SendToCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            try
            {
                var nsend = socket.EndSendTo(ar);
                Success(nsend);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

    }

}
