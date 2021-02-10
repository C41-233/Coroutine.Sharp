using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Waitables.Await
{
    internal sealed class Awaitable : WaitableTask
    {
        public CoroutineManager.Container Container { get; }

        public Awaitable(CoroutineManager.Container container)
        {
            Container = container;
        }

        public new void Success()
        {
            base.Success();
        }

        public new void Fail(Exception e)
        {
            base.Fail(e);
        }

        public void ContinueWith(IAsyncStateMachine state)
        {
            if (Status == WaitableStatus.Running)
            {
                state.MoveNext();
            }
        }

        public void UnsafeContinueWith(IAsyncStateMachine state)
        {
            Container.Manager.Enqueue(() =>
            {
                ContinueWith(state);
            });
        }

    }

    internal sealed class Awaitable<T> : WaitableTask<T>
    {

        public CoroutineManager.Container Container { get; }

        public Awaitable(CoroutineManager.Container container)
        {
            Container = container;
        }

        public new void Success(T result)
        {
            base.Success(result);
        }

        public new void Fail(Exception e)
        {
            base.Fail(e);
        }

        public void ContinueWith(IAsyncStateMachine state)
        {
            if (Status == WaitableStatus.Running)
            {
                state.MoveNext();
            }
        }

        public void UnsafeContinueWith(IAsyncStateMachine state)
        {
            Container.Manager.Enqueue(() =>
            {
                ContinueWith(state);
            });
        }
    }
}
