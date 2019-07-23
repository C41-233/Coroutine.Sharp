﻿using System;
using System.Collections.Generic;
using Coroutine.Base;

namespace Coroutine.Timer
{
    public class TimerManager
    {

        private readonly PriorityQueue<TimerHandle> queue;

        public DateTime Now { get; private set; }

        public Action<Exception> OnUnhandledException { internal get; set; } = DefaultUnhandledException;

        public TimerManager(DateTime startTime)
        {
            Now = startTime;
            queue = new PriorityQueue<TimerHandle>((x, y) => Comparer<DateTime>.Default.Compare(x.At, y.At));
        }

        public TimerHandle StartTimerAfter(TimeSpan span, Action callback)
        {
            return StartTimerAt(Now + span, callback);
        }
        public TimerHandle StartTimerAfter(long milliseconds, Action callback)
        {
            return StartTimerAt(Now.AddMilliseconds(milliseconds), callback);
        }

        public TimerHandle StartTimerAt(DateTime at, Action callback)
        {
            var timer = new TimerHandle(at, callback);
            queue.Enqueue(timer);
            return timer;
        }

        public void Update(DateTime now)
        {
            Now = now;
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

                try
                {
                    timer.Callback();
                }
                catch(Exception e)
                {
                    OnUnhandledException?.Invoke(e);
                }
            }
        }

        private static void DefaultUnhandledException(Exception e)
        {
            Console.Error.WriteLine(e);
        }

    }
}
