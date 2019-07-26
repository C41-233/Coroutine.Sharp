using System;

namespace Coroutines.Await
{
    internal class Awaitable : WaitableTask
    {
        public CoroutineManager.Container CoroutineManager { get; }

        public Awaitable(CoroutineManager.Container coroutineManager)
        {
            CoroutineManager = coroutineManager;
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

    internal class Awaitable<T> : WaitableTask<T>
    {
        public CoroutineManager.Container CoroutineManager { get; }

        public Awaitable(CoroutineManager.Container coroutineManager)
        {
            CoroutineManager = coroutineManager;
        }

        public new void Success(T value)
        {
            base.Success(value);
        }

        public new void Fail(Exception e)
        {
            base.Fail(e);
        }

    }
}
