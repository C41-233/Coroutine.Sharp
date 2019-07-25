using System;
using System.Net;
using System.Net.Sockets;

namespace Coroutines
{

    public struct WaitForReceiveFromResult
    {
        public int Size;
        public EndPoint RemoteEndPoint;
    }

    internal class WaitForReceiveFrom : WaitableTask<WaitForReceiveFromResult>
    {

        public WaitForReceiveFrom(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags)
        {
            try
            {
                var endPoint = socket.LocalEndPoint;
                socket.BeginReceiveFrom(buffer, offset, size, flags, ref endPoint, ReceiveFromCallback, socket);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        private void ReceiveFromCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            try
            {
                var endPoint = socket.LocalEndPoint;
                var nread = socket.EndReceiveFrom(ar, ref endPoint);
                Success(new WaitForReceiveFromResult
                {
                    Size = nread,
                    RemoteEndPoint = endPoint,
                });
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

    }
}
