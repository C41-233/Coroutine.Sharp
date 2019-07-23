using System;

namespace Coroutines
{
    internal class WaitForAllSuccess : WaitableTask
    {

        private readonly IWaitable[] waitables;
        private int countDown;

        public WaitForAllSuccess(IWaitable[] waitables)
        {
            this.waitables = (IWaitable[]) waitables.Clone();
            countDown = waitables.Length;
            foreach (var waitable in waitables)
            {
                waitable.OnSuccess(OnSuccessCallback);
                waitable.OnFail(OnFailCallback);
            }
        }

        private void OnSuccessCallback()
        {
            lock (waitables)
            {
                if (countDown <= 0)
                {
                    return;
                }
                countDown--;
                if (countDown != 0)
                {
                    return;
                }
            }
            Success();
        }

        private void OnFailCallback(Exception e)
        {
            lock (waitables)
            {
                if (countDown <= 0)
                {
                    return;
                }
                countDown = 0;
            }
            foreach (var waitable in waitables)
            {
                if (waitable.Status == WaitableStatus.Running)
                {
                    waitable.Abort();
                }
            }
            Fail(e);
        }

    }
}
