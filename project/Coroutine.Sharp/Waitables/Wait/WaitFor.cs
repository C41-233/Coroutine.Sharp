using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Coroutines.Timers;

namespace Coroutines
{
    public static class WaitFor
    {

        #region Logic

        /// <summary>
        /// 同时等待多个过程，任意一个过程的成功时，当前过程就进入成功状态，并中断其他过程。
        /// </summary>
        /// <returns>第一个成功的Waitable</returns>
        public static IWaitable<IWaitable> AnySuccess(params IWaitable[] waitables)
        {
            return new WaitForAnySuccess(waitables);
        }

        /// <summary>
        /// 等待多个过程，所有过程都成功时，当前过程进入成功状态。
        /// 任意一个过程失败时，当前过程失败，并中断其他过程。
        /// </summary>
        public static IWaitable AllSuccess(params IWaitable[] waitables)
        {
            return new WaitForAllSuccess(waitables);
        }

        /// <summary>
        /// 等待多个过程完成。当所有过程成功时，当前过程进入成功状态；否则进入失败状态。
        /// </summary>
        public static IWaitable All(params IWaitable[] waitables)
        {
            return new WaitForAll(waitables);
        }

        public static IWaitable Promise(Action<Action, Action<Exception>> promise)
        {
            return new WaitForPromise(promise);
        }

        public static IWaitable Yield(int frame = 1)
        {
            return new WaitForFrame(frame);
        }
        #endregion

        #region Time
        public static IWaitable Milliseconds(TimerManager timerManager, long milliseconds)
        {
            return new WaitForTimeSpan(timerManager, TimeSpan.FromMilliseconds(milliseconds));
        }

        public static IWaitable Seconds(TimerManager timerManager, double seconds)
        {
            return new WaitForTimeSpan(timerManager, TimeSpan.FromSeconds(seconds));
        }

        public static IWaitable Minutes(TimerManager timerManager, double minutes)
        {
            return new WaitForTimeSpan(timerManager, TimeSpan.FromMinutes(minutes));
        }

        public static IWaitable Hours(TimerManager timerManager, double hours)
        {
            return new WaitForTimeSpan(timerManager, TimeSpan.FromHours(hours));
        }

        public static IWaitable Time(TimerManager timerManager, TimeSpan timeSpan)
        {
            return new WaitForTimeSpan(timerManager, timeSpan);
        }

        public static IWaitable PhysicalTime(TimeSpan timeSpan)
        {
            return new WaitForPhysicalTimeSpan(timeSpan);
        }
        #endregion

        #region Socket

        public static IWaitable Connect(Socket socket, string host, int port)
        {
            return new WaitForConnect(socket, host, port);
        }

        public static IWaitable Connect(Socket socket, IPAddress address, int port)
        {
            return new WaitForConnect(socket, address, port);
        }

        public static IWaitable Connect(Socket socket, IPAddress[] addresses, int port)
        {
            return new WaitForConnect(socket, addresses, port);
        }

        public static IWaitable Connect(Socket socket, EndPoint endPoint)
        {
            return new WaitForConnect(socket, endPoint);
        }

        public static IWaitable<Socket> Accept(Socket socket)
        {
            return new WaitForAccept(socket);
        }

        public static IWaitable<Socket> Accept(Socket socket, int receiveSize)
        {
            return new WaitForAccept(socket, receiveSize);
        }

        public static IWaitable<Socket> Accept(Socket socket, Socket acceptSocket, int receiveSize)
        {
            return new WaitForAccept(socket, acceptSocket, receiveSize);
        }

        public static IWaitable<int> Receive(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForReceive(socket, buffer, offset, size, flags);
        }

        public static IWaitable<int> Receive(Socket socket, byte[] buffer, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForReceive(socket, buffer, 0, buffer.Length, flags);
        }

        public static IWaitable<int> Receive(Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForReceive(socket, buffers, flags);
        }

        public static IWaitable<WaitForReceiveFromResult> ReceiveFrom(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForReceiveFrom(socket, buffer, offset, size, flags);
        }

        public static IWaitable<WaitForReceiveFromResult> ReceiveFrom(Socket socket, byte[] buffer, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForReceiveFrom(socket, buffer, 0, buffer.Length, flags);
        }

        public static IWaitable<int> Send(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSend(socket, buffer, offset, size, flags);
        }

        public static IWaitable<int> Send(Socket socket, byte[] buffer, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSend(socket, buffer, 0, buffer.Length, flags);
        }

        public static IWaitable<int> Send(Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSend(socket, buffers, flags);
        }

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, int offset, int size, EndPoint endPoint, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSendTo(socket, buffer, offset, size, flags, endPoint);
        }

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, int offset, int size, IPAddress ip, int port, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSendTo(socket, buffer, offset, size, flags, new IPEndPoint(ip, port));
        }

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, int offset, int size, string ip, int port, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSendTo(socket, buffer, offset, size, flags, new IPEndPoint(IPAddress.Parse(ip), port));
        }

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, EndPoint endPoint, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSendTo(socket, buffer, 0, buffer.Length, flags, endPoint);
        }

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, IPAddress ip, int port, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSendTo(socket, buffer, 0, buffer.Length, flags, new IPEndPoint(ip, port));
        }

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, string ip, int port, SocketFlags flags = SocketFlags.None)
        {
            return new WaitForSendTo(socket, buffer, 0, buffer.Length, flags, new IPEndPoint(IPAddress.Parse(ip), port));
        }
        #endregion

    }
}
