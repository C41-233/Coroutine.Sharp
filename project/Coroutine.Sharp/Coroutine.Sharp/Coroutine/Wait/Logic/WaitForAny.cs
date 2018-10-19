using System;
using System.Collections.Generic;

namespace Coroutine.Wait
{
    internal class WaitForAny : WaitableTask<IWaitable>
    {

        private readonly IWaitable[] waitables;
        private readonly List<Exception> exceptions;
        private bool isFinish;
        private int failCount;

        private readonly object barrier = new object();

        public WaitForAny(IWaitable[] waitables)
        {
            exceptions = new List<Exception>(waitables.Length);

            this.waitables = (IWaitable[]) waitables.Clone();

            foreach (var waitable in waitables)
            {
                waitable.OnSuccess(() => OnSuccessCallback(waitable));
                waitable.OnFail(OnFailCallback);
            }
        }

        private void OnSuccessCallback(IWaitable successWaitable)
        {
            lock (barrier)
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
            lock (barrier)
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
