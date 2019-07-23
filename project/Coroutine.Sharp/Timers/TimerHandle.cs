using System;

namespace Coroutines.Timers
{
    public sealed class TimerHandle
    {

        public DateTime At { get; }
        internal Action Callback { get; private set; }
        public bool IsStopped { get; private set; }

        internal TimerHandle(DateTime at, Action callback)
        {
            At = at;
            Callback = callback;
        }

        public void Stop()
        {
            IsStopped = true;
            Callback = null;
        }

    }
}
