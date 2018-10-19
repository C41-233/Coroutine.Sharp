using System;

namespace Coroutine.Wait
{
    internal abstract class FilterWaitable : IWaitable
    {

        protected readonly IWaitable Waitable;

        protected FilterWaitable(IWaitable waitable)
        {
            Waitable = waitable;
        }

        public virtual WaitableStatus Status => Waitable.Status;

        public virtual Exception Exception => Waitable.Exception;

        public virtual IWaitable OnSuccess(Action callback)
        {
            Waitable.OnSuccess(callback);
            return this;
        }

        public virtual IWaitable OnFail(Action<Exception> callback)
        {
            Waitable.OnFail(callback);
            return this;
        }

        public virtual void Abort()
        {
            Waitable.Abort();
        }

    }
}
