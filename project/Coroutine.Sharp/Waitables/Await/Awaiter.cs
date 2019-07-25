using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Await
{
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
            Console.WriteLine("UnsafeOnCompleted");
            waitable.Finally(continuation);
        }

        public bool IsCompleted => waitable.IsFinish();

        public void GetResult()
        {
        }
    }

    public struct CoroutineAwaiter : ICriticalNotifyCompletion
    {

        private readonly Coroutine coroutine;

        public CoroutineAwaiter(Coroutine coroutine)
        {
            this.coroutine = coroutine;
        }

        public void OnCompleted(Action continuation)
        {
            coroutine.Finally(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            coroutine.Finally(continuation);
        }

        public bool IsCompleted => coroutine.IsFinish();

        public void GetResult()
        {
        }
    }
}
