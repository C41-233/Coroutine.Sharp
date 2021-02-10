namespace Coroutines.Waitables
{
    internal sealed class Awaitable : WaitableTask
    {
        public CoroutineManager.Container Container { get; }

        public Awaitable(CoroutineManager.Container container)
        {
            Container = container;
        }

    }
}
