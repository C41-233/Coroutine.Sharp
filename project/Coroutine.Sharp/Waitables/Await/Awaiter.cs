using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Await
{
    public struct Awaiter : ICriticalNotifyCompletion
    {

        internal readonly IWaitable waitable;

        public Awaiter(IWaitable waitable)
        {
            this.waitable = waitable;
        }

        public void OnCompleted(Action continuation)
        {
            waitable.Finally(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            waitable.Finally(continuation);
        }

        public bool IsCompleted => waitable.IsCompleted();

        public void GetResult()
        {
            waitable.Throw();
        }
    }

    public struct Awaiter<T> : ICriticalNotifyCompletion
    {

        internal readonly IWaitable<T> waitable;

        public Awaiter(IWaitable<T> waitable)
        {
            this.waitable = waitable;
        }

        public void OnCompleted(Action continuation)
        {
            waitable.Finally(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            waitable.Finally(continuation);
        }

        public bool IsCompleted => waitable.IsCompleted();

        public T GetResult()
        {
            return waitable.R;
        }
    }

}
