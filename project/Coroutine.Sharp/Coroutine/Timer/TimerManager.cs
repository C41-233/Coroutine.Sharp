using System;
using Coroutine.Base;

namespace Coroutine.Timer
{
    public class TimerManager
    {

        private readonly PriorityQueue<TimerHandle> queue;
        private long now;

        public TimerManager(long timestamp)
        {
            now = timestamp;
            queue = new PriorityQueue<TimerHandle>();
        }

        public TimerHandle StartTimerAfter(long after, Action callback)
        {
            var timer = new TimerHandle(now + after, callback);
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
            now = timestamp;
            while (queue.Count > 0)
            {
                var timer = queue.Top;
                if (timer.IsStopped)
                {
                    queue.Dequeue();
                    continue;
                }
                if (timer.At > now)
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
