using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Coroutines.Base;

namespace Coroutines.Await
{

    internal struct AwaitShareData
    {
        public CoroutineManager.Container Container;
        public DebugInfo DebugInfo;
    }

    internal static class AwaitShareDataStatic
    {

        [ThreadStatic]
        internal static AwaitShareData Share;

        internal static void Dispatch<TAwaiter, TStateMachine>(CoroutineManager manager, ref TAwaiter awaiter, TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            awaiter.OnCompleted(() =>
            {
                if (tid == Thread.CurrentThread.ManagedThreadId)
                {
                    stateMachine.MoveNext();
                }
                else
                {
                    manager.Enqueue(stateMachine.MoveNext);
                }
            });
        }

    }

    public struct AwaitMethodBuilder
    {

        public static AwaitMethodBuilder Create()
        {
            var share = AwaitShareDataStatic.Share;
            if (share.Container == null)
            {
                throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine instead.");
            }

            var builder = new AwaitMethodBuilder(new Awaitable(share.Container, share.DebugInfo));
            AwaitShareDataStatic.Share = default;
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
            }

            AwaitShareDataStatic.Dispatch(manager, ref awaiter, stateMachine);
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
            var share = AwaitShareDataStatic.Share;
            if (share.Container == null)
            {
                throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine<T> instead.");
            }

            var builder = new AwaitMethodBuilder<T>(new Awaitable<T>(share.Container, share.DebugInfo));
            AwaitShareDataStatic.Share = default;
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
            }

            AwaitShareDataStatic.Dispatch(manager, ref awaiter, stateMachine);
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
