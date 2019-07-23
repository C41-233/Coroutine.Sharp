using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Coroutines.Wait
{
    internal class WaitForReceive : WaitableTask<int>
    {

        public WaitForReceive(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags)
        {
            try
            {
                socket.BeginReceive(buffer, offset, size, flags, ReceiveCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        public WaitForReceive(Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags flags)
        {
            try
            {
                socket.BeginReceive(buffers, flags, ReceiveCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            try
            {
                int nread = socket.EndReceive(ar);
                Success(nread);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

    }
}
