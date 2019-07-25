using System;
using System.Collections.Generic;

namespace Coroutines
{
    internal class WaitForAnySuccess : WaitableTask<IWaitable>
    {

        private readonly IWaitable[] waitables;
        private readonly List<Exception> exceptions;
        private bool isFinish;
        private int failCount;

        public WaitForAnySuccess(IWaitable[] waitables)
        {
            exceptions = new List<Exception>(waitables.Length);

            this.waitables = (IWaitable[]) waitables.Clone();

            foreach (var waitable in waitables)
            {
                waitable.Then(() => OnSuccessCallback(waitable));
                waitable.Catch(OnFailCallback);
            }
        }

        private void OnSuccessCallback(IWaitable successWaitable)
        {
            lock (waitables)
            {
                if (isFinish)
                {
                    return;
                }
                isFinish = true;
            }

            foreach (var waitable in waitables)
            {
                if (waitable.Status == WaitableStatus.Running)
                {
                    waitable.Abort();
                }
            }

            Success(successWaitable);
        }

        private void OnFailCallback(Exception e)
        {
            lock (waitables)
            {
                if (isFinish)
                {
                    return;
                }
                failCount++;
                exceptions.Add(e);
                if (failCount < waitables.Length)
                {
                    return;
                }
                isFinish = true;
            }
            Fail(new AggregateException(exceptions));
        }

    }
}
