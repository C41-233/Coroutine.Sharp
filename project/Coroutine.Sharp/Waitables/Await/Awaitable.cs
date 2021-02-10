using System;

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

    }
}
