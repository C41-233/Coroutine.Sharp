using System;
using Coroutine.Timer;

namespace Coroutine.Wait
{

    internal class WaitForTimeSpan : WaitableTask
    {

        private readonly TimerHandle timer;

        public WaitForTimeSpan(TimerManager timerManager, TimeSpan timeSpan)
        {
            timer = timerManager.StartTimerAfter(timeSpan, Success);
        }

        protected override void OnAbort()
        {
            timer.Stop();
        }
    }

}
