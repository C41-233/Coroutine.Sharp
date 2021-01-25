using System;

namespace Coroutines
{
    internal class WaitForAllSuccess : WaitableTask
    {

        private readonly IWaitable[] waitables;
        private readonly bool abortOthers;
        private int countDown;

        public WaitForAllSuccess(IWaitable[] waitables, bool abortOthers)
        {
            this.waitables = (IWaitable[]) waitables.Clone();
            this.abortOthers = abortOthers;

            countDown = waitables.Length;
            foreach (var waitable in waitables)
            {
                waitable.Then(OnSuccessCallback);
                waitable.Catch(OnFailCallback);
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

            if (abortOthers)
            {
                foreach (var waitable in waitables)
                {
                    if (waitable.Status == WaitableStatus.Running)
                    {
                        waitable.Abort();
                    }
                }
            }
            Fail(e);
        }

        protected override void OnAbort(bool recursive)
        {
            if (recursive)
            {
                foreach (var waitable in waitables)
                {
                    if (waitable.Status == WaitableStatus.Running)
                    {
                        waitable.Abort();
                    }
                }
            }
        }
    }
}
