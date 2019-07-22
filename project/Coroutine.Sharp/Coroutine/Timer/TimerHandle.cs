using System;
using System.Collections.Generic;

namespace Coroutine.Timer
{
    public class TimerHandle : IComparable<TimerHandle>
    {

        public long At { get; }
        public Action Callback { get; }
        public bool IsStopped { get; private set; }

        internal TimerHandle(long at, Action callback)
        {
            At = at;
            Callback = callback;
        }

        public void Stop()
        {
            IsStopped = true;
        }

        int IComparable<TimerHandle>.CompareTo(TimerHandle other)
        {
            return Comparer<long>.Default.Compare(At, other.At);
        }
    }
}
