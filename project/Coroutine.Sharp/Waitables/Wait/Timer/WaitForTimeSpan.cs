using System;
using Coroutines.Timers;

namespace Coroutines
{

    internal class WaitForTimeSpan : WaitableTask
    {

        private readonly TimerHandle timer;

        public WaitForTimeSpan(TimerManager timerManager, TimeSpan timeSpan)
        {
            timer = timerManager.StartTimerAfter(timeSpan, Success);
        }

        protected override void OnAbort(bool recursive)
        {
            timer.Stop();
        }
    }

}
