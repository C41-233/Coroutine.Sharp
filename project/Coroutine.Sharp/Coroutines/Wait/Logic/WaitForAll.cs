using System;
using System.Collections.Generic;

namespace Coroutines.Wait
{
    internal class WaitForAll : WaitableTask
    {

        private readonly List<Exception> exceptions;
        private int countDown;

        public WaitForAll(IWaitable[] waitables)
        {
            exceptions = new List<Exception>(waitables.Length);
            countDown = waitables.Length;
            foreach (var waitable in waitables)
            {
                waitable.OnSuccess(OnSuccessCallback);
                waitable.OnFail(OnFailCallback);
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

    }
}
