using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Waitables.Await
{

    public static class AwaiterExtend
    {
        public static Awaiter GetAwaiter(this IWaitable waitable)
        {
            return new Awaiter(waitable);
        }
    }

    public struct Awaiter : ICriticalNotifyCompletion
    {

        private readonly IWaitable waitable;

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
}
