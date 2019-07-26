namespace Coroutines
{

    public sealed class WaitForFrame : WaitableTask, IBindCoroutineWaitable
    {

        private CoroutineManager coroutineManager;
        private int n;

        public WaitForFrame(int n)
        {
            this.n = n;
        }

        void IBindCoroutineWaitable.Bind(CoroutineManager.Container container)
        {
            coroutineManager = container.CoroutineManager;
            coroutineManager.Enqueue(NextFrame);
        }

        public void NextFrame()
        {
            n--;
            if (n == 0)
            {
                Success();
            }
            else
            {
                coroutineManager.Enqueue(NextFrame);
            }
        }

    }

}
