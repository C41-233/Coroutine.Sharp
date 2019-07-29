using System;

namespace Coroutines.Await
{

    internal class Awaitable : WaitableTask
    {
        public CoroutineManager.Container CoroutineContainer { get; }

        public Awaitable(CoroutineManager.Container container)
        {
            CoroutineContainer = container;
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
        public CoroutineManager.Container CoroutineContainer { get; }

        public Awaitable(CoroutineManager.Container container)
        {
            CoroutineContainer = container;
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
