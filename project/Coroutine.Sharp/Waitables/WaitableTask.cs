using System;
using System.Runtime.ExceptionServices;

namespace Coroutines.Waitables
{
    public abstract class WaitableTask : Waitable
    {

        protected new void Success()
        {
            base.Success();
        }

        protected new void Fail(Exception e)
        {
            base.Fail(e);
        }

    }

    public abstract class WaitableTask<T> : Waitable, IWaitable<T>
    {

        public T Result
        {
            get
            {
                this.Throw();
                return result;
            }
        }

        private T result;

        protected void Success(T result)
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }
            this.result = result;
            Success();
        }

        protected new void Fail(Exception e)
        {
            base.Fail(e);
        }

    }
}
