using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Waitables.Await
{
    internal interface IAwaitable : IWaitable
    {

        CoroutineManager.Container Container { get; }

    }

    internal sealed class Awaitable : WaitableTask, IAwaitable
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

    }

    internal sealed class Awaitable<T> : WaitableTask<T>, IAwaitable
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

    }

    internal static class ContinueWithExtends
    {

        public static void ContinueWith(this IAwaitable self, IAsyncStateMachine state)
        {
            switch (self.Status)
            {
                case WaitableStatus.Running:
                    state.MoveNext();
                    break;
                case WaitableStatus.Success:
                    break;
                case WaitableStatus.Abort:
                case WaitableStatus.Error:
                    self.Throw();
                    break;
            }
        }

        public static void UnsafeContinueWith(this IAwaitable self, IAsyncStateMachine state)
        {
            self.Container.Manager.Enqueue(() =>
            {
                ContinueWith(self, state);
            });
        }
    }

}
