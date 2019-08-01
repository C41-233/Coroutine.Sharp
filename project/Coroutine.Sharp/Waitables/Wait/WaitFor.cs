using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Coroutines.Signals;
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
        public static IWaitable<IWaitable> AnySuccess(params IWaitable[] waitables) => new WaitForAnySuccess(waitables);

        /// <summary>
        /// 等待多个过程，所有过程都成功时，当前过程进入成功状态。
        /// 任意一个过程失败时，当前过程失败，并中断其他过程。
        /// </summary>
        public static IWaitable AllSuccess(params IWaitable[] waitables) => new WaitForAllSuccess(waitables);

        /// <summary>
        /// 等待多个过程完成。当所有过程成功时，当前过程进入成功状态；否则进入失败状态。
        /// </summary>
        public static IWaitable All(params IWaitable[] waitables) => new WaitForAll(waitables);

        public static IWaitable Promise(Action<Action, Action<Exception>> promise) => new WaitForPromise(promise);

        public static IWaitable Yield(int frame = 1) => new WaitForFrame(frame);

        public static IWaitable Task(Task task) => new WaitForTask(task);

        public static IWaitable<T> Task<T>(Task<T> task) => new WaitForTask<T>(task);

        public static IWaitable<T> Signal<T>(SignalManager.Container container, Predicate<T> predicate) => new WaitForSignal<T>(container, predicate);

        public static IWaitable<T> Signal<T>(SignalManager.Container container) => new WaitForSignal<T>(container, null);
        #endregion

        #region Time
        public static IWaitable Milliseconds(TimerManager timerManager, long milliseconds) => new WaitForTimeSpan(timerManager, TimeSpan.FromMilliseconds(milliseconds));

        public static IWaitable Seconds(TimerManager timerManager, double seconds) => new WaitForTimeSpan(timerManager, TimeSpan.FromSeconds(seconds));

        public static IWaitable Minutes(TimerManager timerManager, double minutes) => new WaitForTimeSpan(timerManager, TimeSpan.FromMinutes(minutes));

        public static IWaitable Hours(TimerManager timerManager, double hours) => new WaitForTimeSpan(timerManager, TimeSpan.FromHours(hours));

        public static IWaitable Time(TimerManager timerManager, TimeSpan timeSpan) => new WaitForTimeSpan(timerManager, timeSpan);

        public static IWaitable PhysicalTime(TimeSpan timeSpan) => new WaitForPhysicalTimeSpan(timeSpan);

        #endregion

        #region Socket

        public static IWaitable Connect(Socket socket, string host, int port) => new WaitForConnect(socket, host, port);

        public static IWaitable Connect(Socket socket, IPAddress address, int port) => new WaitForConnect(socket, address, port);

        public static IWaitable Connect(Socket socket, IPAddress[] addresses, int port) => new WaitForConnect(socket, addresses, port);

        public static IWaitable Connect(Socket socket, EndPoint endPoint) => new WaitForConnect(socket, endPoint);

        public static IWaitable<Socket> Accept(Socket socket) => new WaitForAccept(socket);

        public static IWaitable<Socket> Accept(Socket socket, int receiveSize) => new WaitForAccept(socket, receiveSize);

        public static IWaitable<Socket> Accept(Socket socket, Socket acceptSocket, int receiveSize) => new WaitForAccept(socket, acceptSocket, receiveSize);

        public static IWaitable<int> Receive(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags = SocketFlags.None) => new WaitForReceive(socket, buffer, offset, size, flags);

        public static IWaitable<int> Receive(Socket socket, byte[] buffer, SocketFlags flags = SocketFlags.None) => new WaitForReceive(socket, buffer, 0, buffer.Length, flags);

        public static IWaitable<int> Receive(Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags flags = SocketFlags.None) => new WaitForReceive(socket, buffers, flags);

        public static IWaitable<WaitForReceiveFromResult> ReceiveFrom(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags = SocketFlags.None) => new WaitForReceiveFrom(socket, buffer, offset, size, flags);

        public static IWaitable<WaitForReceiveFromResult> ReceiveFrom(Socket socket, byte[] buffer, SocketFlags flags = SocketFlags.None) => new WaitForReceiveFrom(socket, buffer, 0, buffer.Length, flags);

        public static IWaitable<int> Send(Socket socket, byte[] buffer, int offset, int size, SocketFlags flags = SocketFlags.None) => new WaitForSend(socket, buffer, offset, size, flags);

        public static IWaitable<int> Send(Socket socket, byte[] buffer, SocketFlags flags = SocketFlags.None) => new WaitForSend(socket, buffer, 0, buffer.Length, flags);

        public static IWaitable<int> Send(Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags flags = SocketFlags.None) => new WaitForSend(socket, buffers, flags);

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, int offset, int size, EndPoint endPoint, SocketFlags flags = SocketFlags.None) => new WaitForSendTo(socket, buffer, offset, size, flags, endPoint);

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, int offset, int size, IPAddress ip, int port, SocketFlags flags = SocketFlags.None) => new WaitForSendTo(socket, buffer, offset, size, flags, new IPEndPoint(ip, port));

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, int offset, int size, string ip, int port, SocketFlags flags = SocketFlags.None) => new WaitForSendTo(socket, buffer, offset, size, flags, new IPEndPoint(IPAddress.Parse(ip), port));

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, EndPoint endPoint, SocketFlags flags = SocketFlags.None) => new WaitForSendTo(socket, buffer, 0, buffer.Length, flags, endPoint);

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, IPAddress ip, int port, SocketFlags flags = SocketFlags.None) => new WaitForSendTo(socket, buffer, 0, buffer.Length, flags, new IPEndPoint(ip, port));

        public static IWaitable<int> SendTo(Socket socket, byte[] buffer, string ip, int port, SocketFlags flags = SocketFlags.None) => new WaitForSendTo(socket, buffer, 0, buffer.Length, flags, new IPEndPoint(IPAddress.Parse(ip), port));

        #endregion

        #region IO
        public static IWaitable<int> Read(Stream stream, byte[] buffer, int offset, int count) => new WaitForRead(stream, buffer, offset, count);

        public static IWaitable<int> Read(Stream stream, byte[] buffer) => new WaitForRead(stream, buffer, 0, buffer.Length);

        #endregion

    }
}
