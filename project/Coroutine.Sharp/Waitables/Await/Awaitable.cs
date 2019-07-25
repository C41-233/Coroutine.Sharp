namespace Coroutines.Await
{
    internal class Awaitable : WaitableTask
    {
        public CoroutineManager CoroutineManager { get; }

        public Awaitable(CoroutineManager coroutineManager)
        {
            CoroutineManager = coroutineManager;
        }

        public void Complete()
        {
            Success();
        }

    }
}
