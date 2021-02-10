using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    internal sealed class AsyncMethodBuilderAttribute : Attribute
    {
        // ReSharper disable once UnusedParameter.Local
        public AsyncMethodBuilderAttribute(Type type)
        { }
    }
}

namespace Coroutines.Waitables.Await
{

    internal readonly struct AwaitShareData
    {
        public readonly CoroutineManager.Container Container;

        public AwaitShareData(CoroutineManager.Container container)
        {
            Container = container;
        }
    }

    internal static class AwaitShareDataStatic
    {

        [ThreadStatic]
        internal static AwaitShareData? Share;

    }

    public struct AwaitMethodBuilder
    {

        public static AwaitMethodBuilder Create()
        {
            var share = AwaitShareDataStatic.Share;
            if (share == null)
            {
                throw WaitableFlowException.AsyncCallDirectly();
            }

            AwaitShareDataStatic.Share = null;
            return new AwaitMethodBuilder(share.Value);
        }

        private readonly Awaitable waitable;

        private AwaitMethodBuilder(in AwaitShareData share)
        {
            waitable = new Awaitable(share.Container);
        }

        public IWaitable Task => waitable;

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var state = stateMachine;
            var waitable = this.waitable;
            awaiter.OnCompleted(() =>
            {
                waitable.ContinueWith(state);
            });
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var state = stateMachine;
            var waitable = this.waitable;
            awaiter.OnCompleted(() =>
            {
                waitable.UnsafeContinueWith(state);
            });
        }

        public void SetResult()
        {
            waitable.Success();
        }

        public void SetException(Exception e)
        {
            waitable.Fail(e);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

    }

    public struct AwaitMethodBuilder<T>
    {

        public static AwaitMethodBuilder<T> Create()
        {
            var share = AwaitShareDataStatic.Share;
            if (share == null)
            {
                throw WaitableFlowException.AsyncCallDirectly();
            }

            AwaitShareDataStatic.Share = null;
            return new AwaitMethodBuilder<T>(share.Value);
        }

        private readonly Awaitable<T> waitable;

        private AwaitMethodBuilder(in AwaitShareData share)
        {
            waitable = new Awaitable<T>(share.Container);
        }

        public IWaitable<T> Task => waitable;

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var state = stateMachine;
            var waitable = this.waitable;
            awaiter.OnCompleted(() =>
            {
                waitable.ContinueWith(state);
            });
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var state = stateMachine;
            var waitable = this.waitable;
            awaiter.UnsafeOnCompleted(() =>
            {
                waitable.UnsafeContinueWith(state);
            });
        }

        public void SetResult(T result)
        {
            waitable.Success(result);
        }

        public void SetException(Exception e)
        {
            waitable.Fail(e);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

    }
}
