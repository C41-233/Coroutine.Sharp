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
        }

        public void UnsafeOnCompleted(Action continuation)
        {
        }

        public bool IsCompleted => true;

        public void GetResult()
        {
        }

    }
}
