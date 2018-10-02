using Coroutine.Timer;

namespace Coroutine.Wait
{

    internal class WaitForMilliseconds : WaitableTask
    {

        private readonly TimerHandle timer;

        public WaitForMilliseconds(TimerManager timerManager, long milliseconds)
        {
            timer = timerManager.StartTimerAfter(milliseconds, Success);
        }

        protected override void OnAbort()
        {
            timer.Stop();
        }
    }

}
