using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Await
{

    internal static class AwaitShareData
    {

        [ThreadStatic]
        internal static CoroutineManager.Container ThreadLocalCoroutineContainer;

    }

    public struct AwaitMethodBuilder
    {

        public static AwaitMethodBuilder Create()
        {
            var coroutineManager = AwaitShareData.ThreadLocalCoroutineContainer;
            if (coroutineManager == null)
            {
                throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine instead.");
            }

            var builder = new AwaitMethodBuilder(new Awaitable(coroutineManager));
            AwaitShareData.ThreadLocalCoroutineContainer = null;
            return builder;
        }

        public IWaitable Task => awaitable;

        private readonly Awaitable awaitable;

        private CoroutineManager.Container coroutineManager => awaitable.CoroutineManager;

        private AwaitMethodBuilder(Awaitable awaitable)
        {
            this.awaitable = awaitable;
        }

        public void SetResult() => awaitable.Success();

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var s = stateMachine;
            var l = coroutineManager.CoroutineManager;
            awaiter.OnCompleted(
                () => l.Enqueue(
                    () => s.MoveNext()
                )
            );
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var s = stateMachine;
            var l = coroutineManager.CoroutineManager;
            if (awaiter is Awaiter w && w.waitable is IBindCoroutineWaitable waitForFrame)
            {
                waitForFrame.Bind(coroutineManager);
            }
            awaiter.UnsafeOnCompleted(
                () => l.Enqueue(
                    () => s.MoveNext()
                )
            );
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
            var coroutineManager = AwaitShareData.ThreadLocalCoroutineContainer;
            if (coroutineManager == null)
            {
                throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine<T> instead.");
            }

            var builder = new AwaitMethodBuilder<T>(new Awaitable<T>(coroutineManager));
            AwaitShareData.ThreadLocalCoroutineContainer = null;
            return builder;
        }

        public IWaitable<T> Task => awaitable;

        private readonly Awaitable<T> awaitable;

        private CoroutineManager.Container coroutineManager => awaitable.CoroutineManager;

        private AwaitMethodBuilder(Awaitable<T> awaitable)
        {
            this.awaitable = awaitable;
        }

        public void SetResult(T value) => awaitable.Success(value);

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var s = stateMachine;
            var l = coroutineManager.CoroutineManager;
            awaiter.OnCompleted(
                () => l.Enqueue(
                    () => s.MoveNext()
                )
            );
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var s = stateMachine;
            var l = coroutineManager.CoroutineManager;
            if (awaiter is Awaiter w && w.waitable is IBindCoroutineWaitable waitForFrame)
            {
                waitForFrame.Bind(coroutineManager);
            }
            awaiter.UnsafeOnCompleted(
                () => l.Enqueue(
                    () => s.MoveNext()
                )
            );
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
