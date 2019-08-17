using System;
using System.Collections.Generic;

namespace Coroutines
{
    internal class WaitForAll : WaitableTask
    {
        private readonly IWaitable[] waitables;
        private readonly List<Exception> exceptions;
        private int countDown;

        public WaitForAll(IWaitable[] waitables)
        {
            this.waitables = (IWaitable[]) waitables.Clone();
            exceptions = new List<Exception>(waitables.Length);
            countDown = waitables.Length;
            foreach (var waitable in waitables)
            {
                waitable.Then(OnSuccessCallback);
                waitable.Catch(OnFailCallback);
            }
        }

        private void OnSuccessCallback()
        {
            lock (exceptions)
            {
                countDown--;
                if (countDown != 0)
                {
                    return;
                }
            }
            Done();
        }

        private void OnFailCallback(Exception e)
        {
            lock (exceptions)
            {
                exceptions.Add(e);
                countDown--;
                if (countDown != 0)
                {
                    return;
                }
            }
            Done();
        }

        private void Done()
        {
            if (exceptions.Count == 0)
            {
                Success();
            }
            else
            {
                Fail(new AggregateException(exceptions));
            }
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
