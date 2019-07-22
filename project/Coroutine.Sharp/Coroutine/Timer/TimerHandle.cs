using System;

namespace Coroutine.Timer
{
    public class TimerHandle
    {

        public DateTime At { get; }
        public Action Callback { get; }
        public bool IsStopped { get; private set; }

        internal TimerHandle(DateTime at, Action callback)
        {
            At = at;
            Callback = callback;
        }

        public void Stop()
        {
            IsStopped = true;
        }

    }
}
