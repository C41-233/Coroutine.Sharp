using System.Collections.Generic;

namespace Coroutines.Base
{
    internal class SwapQueue<T>
    {

        private Queue<T> inQueue = new Queue<T>();
        private Queue<T> outQueue = new Queue<T>();

        private readonly object syncRoot = new object();

        public void Enqueue(T value)
        {
            lock (syncRoot)
            {
                inQueue.Enqueue(value);
            }
        }

        public void Swap()
        {
            lock (syncRoot)
            {
                var tmp = inQueue;
                inQueue = outQueue;
                outQueue = tmp;
            }
        }

        public int Count => outQueue.Count;

        public T Dequeue => outQueue.Dequeue();

    }
}
