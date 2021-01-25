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

    internal struct AwaitShareData
    {
        internal CoroutineManager.Container Container;
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
                throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine instead.");
            }

            AwaitShareDataStatic.Share = null;
            return new AwaitMethodBuilder(share.Value);
        }

        private readonly Waitable waitable;
        private readonly CoroutineManager.Container container;

        private AwaitMethodBuilder(AwaitShareData share)
        {
            waitable = new Waitable(share.Container);
            container = share.Container;
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
            awaiter.OnCompleted(() =>
            {
                state.MoveNext();
            });
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var state = stateMachine;
            var manager = container.Manager;
            awaiter.UnsafeOnCompleted(() =>
            {
                manager.Enqueue(() => state.MoveNext());
            });
        }

        public void SetResult()
        {
            Console.WriteLine("SetResult");
        }

        public void SetException(Exception e)
        {
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

    }
}
