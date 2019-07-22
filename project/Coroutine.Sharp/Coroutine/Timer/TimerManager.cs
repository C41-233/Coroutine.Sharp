using System;
using Coroutine.Base;

namespace Coroutine.Timer
{
    public class TimerManager
    {

        private readonly PriorityQueue<TimerHandle> queue;

        public long Now { get; private set; }

        public TimerManager(long timestamp)
        {
            Now = timestamp;
            queue = new PriorityQueue<TimerHandle>();
        }

        public TimerHandle StartTimerAfter(long after, Action callback)
        {
            var timer = new TimerHandle(Now + after, callback);
            queue.Enqueue(timer);
            return timer;
        }

        public TimerHandle StartTimerAt(long at, Action callback)
        {
            var timer = new TimerHandle(at, callback);
            queue.Enqueue(timer);
            return timer;
        }

        public void Update(long timestamp)
        {
            Now = timestamp;
            while (queue.Count > 0)
            {
                var timer = queue.Top;
                if (timer.IsStopped)
                {
                    queue.Dequeue();
                    continue;
                }
                if (timer.At > Now)
                {
                    break;
                }
                timer.Stop();
                queue.Dequeue();

                timer.Callback();
            }
        }

    }
}
