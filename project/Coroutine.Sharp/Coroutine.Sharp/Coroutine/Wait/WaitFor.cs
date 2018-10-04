using System.Net;
using System.Net.Sockets;
using Coroutine.Base;
using Coroutine.Timer;

namespace Coroutine.Wait
{
    public static class WaitFor
    {

        public static IWaitable Milliseconds(TimerManager timerManager, long milliseconds)
        {
            return new WaitForMilliseconds(timerManager, milliseconds);
        }

        public static IWaitable Seconds(TimerManager timerManager, long seconds)
        {
            return Milliseconds(timerManager, seconds * TimeUnit.Seconds);
        }

        public static IWaitable Seconds(TimerManager timerManager, double seconds)
        {
            return Milliseconds(timerManager, (long)(seconds * TimeUnit.Seconds));
        }

        public static IWaitable Connect(Socket socket, string host, int port)
        {
            return new WaitForBeginConnect(socket, host, port);
        }

        public static IWaitable Connect(Socket socket, IPAddress address, int port)
        {
            return new WaitForBeginConnect(socket, address, port);
        }

        public static IWaitable Connect(Socket socket, IPAddress[] addresses, int port)
        {
            return new WaitForBeginConnect(socket, addresses, port);
        }

        public static IWaitable Connect(Socket socket, EndPoint endPoint)
        {
            return new WaitForBeginConnect(socket, endPoint);
        }

    }
}
