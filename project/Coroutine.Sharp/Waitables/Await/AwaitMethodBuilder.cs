using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Await
{

    internal static class AwaitShareData
    {

        [ThreadStatic]
        internal static CoroutineManager.Container ThreadLocalCoroutineContainer;

        internal static void FastOnComplete<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        internal static void UnsafeOnComplete<TAwaiter, TStateMachine>(CoroutineManager manager, ref TAwaiter awaiter, TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(() => manager.Enqueue(stateMachine.MoveNext));
        }

    }

    public struct AwaitMethodBuilder
    {

        public static AwaitMethodBuilder Create()
        {
            var container = AwaitShareData.ThreadLocalCoroutineContainer;
            if (container == null)
            {
                throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine instead.");
            }

            var builder = new AwaitMethodBuilder(new Awaitable(container));
            AwaitShareData.ThreadLocalCoroutineContainer = null;
            return builder;
        }

        public IWaitable Task => awaitable;

        private readonly Awaitable awaitable;

        private CoroutineManager.Container container => awaitable.CoroutineContainer;

        private CoroutineManager manager => container.CoroutineManager;

        private AwaitMethodBuilder(Awaitable awaitable)
        {
            this.awaitable = awaitable;
        }

        public void SetResult() => awaitable.Success();

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            var s = stateMachine;
            manager.Enqueue(() => s.MoveNext());
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var waitable = AwaiterStatic<TAwaiter>.GetWaitable?.Invoke(ref awaiter);

            if (waitable is IBindCoroutineWaitable bindCoroutineWaitable)
            {
                bindCoroutineWaitable.Bind(container);
                AwaitShareData.FastOnComplete(ref awaiter, ref stateMachine);
                return;
            }

            AwaitShareData.UnsafeOnComplete(manager, ref awaiter, stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetException(Exception e)
        {
            awaitable.Fail(e);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Console.WriteLine("SetStateMachine");
        }

    }

    public struct AwaitMethodBuilder<T>
    {

        public static AwaitMethodBuilder<T> Create()
        {
            var container = AwaitShareData.ThreadLocalCoroutineContainer;
            if (container == null)
            {
                throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine<T> instead.");
            }

            var builder = new AwaitMethodBuilder<T>(new Awaitable<T>(container));
            AwaitShareData.ThreadLocalCoroutineContainer = null;
            return builder;
        }

        public IWaitable<T> Task => awaitable;

        private readonly Awaitable<T> awaitable;

        private CoroutineManager.Container container => awaitable.CoroutineContainer;

        private CoroutineManager manager => container.CoroutineManager;

        private AwaitMethodBuilder(Awaitable<T> awaitable)
        {
            this.awaitable = awaitable;
        }

        public void SetResult(T value) => awaitable.Success(value);

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            var s = stateMachine;
            manager.Enqueue(() => s.MoveNext());
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var waitable = AwaiterStatic<TAwaiter>.GetWaitable?.Invoke(ref awaiter);
            if (waitable is IBindCoroutineWaitable bindCoroutineWaitable)
            {
                bindCoroutineWaitable.Bind(container);
                AwaitShareData.FastOnComplete(ref awaiter, ref stateMachine);
                return;
            }
            AwaitShareData.UnsafeOnComplete(manager, ref awaiter, stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetException(Exception e)
        {
            awaitable.Fail(e);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Console.WriteLine("SetStateMachine");
        }

    }

}
