using System;

namespace Coroutines.Await
{
    internal class Awaitable : WaitableTask
    {
        public CoroutineManager CoroutineManager { get; }

        public Awaitable(CoroutineManager coroutineManager)
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
}
