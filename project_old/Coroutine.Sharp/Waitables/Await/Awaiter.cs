using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Await
{

    internal delegate IWaitable GetWaitableDelegate<TAwaiter>(ref TAwaiter awaiter) where TAwaiter : INotifyCompletion;

    //防止awaiter装箱
    internal static class AwaiterStatic<TAwaiter> where TAwaiter : INotifyCompletion
    {
        public static GetWaitableDelegate<TAwaiter> GetWaitable;
    }

    public struct Awaiter : ICriticalNotifyCompletion
    {

        static Awaiter()
        {
            AwaiterStatic<Awaiter>.GetWaitable = (ref Awaiter awaiter) => awaiter.waitable;
        }

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

        static Awaiter()
        {
            AwaiterStatic<Awaiter<T>>.GetWaitable = (ref Awaiter<T> awaiter) => awaiter.waitable;
        }

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
