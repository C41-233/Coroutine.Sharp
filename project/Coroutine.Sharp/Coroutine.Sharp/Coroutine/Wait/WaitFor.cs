using Coroutine.Timer;

namespace Coroutine.Wait
{
    public static class WaitFor
    {

        public static WaitableTask Milliseconds(TimerManager timerManager, long milliseconds)
        {
            return new WaitForMilliseconds(timerManager, milliseconds);
        }

    }
}
