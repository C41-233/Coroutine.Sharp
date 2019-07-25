using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Coroutines
{
    internal class WaitForSend : WaitableTask<int>
    {

        public WaitForSend(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags)
        {
            try
            {
                socket.BeginSend(buffer, offset, size, flags, SendCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        public WaitForSend(Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags flags)
        {
            try
            {
                socket.BeginSend(buffers, flags, SendCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            try
            {
                var nsend = socket.EndSend(ar);
                Success(nsend);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

    }
}
