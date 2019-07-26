using System.Collections.Generic;

namespace Coroutines.Base
{
    internal class SwapQueue<T>
    {

        private Queue<T> inQueue = new Queue<T>();
        private Queue<T> outQueue = new Queue<T>();

        private readonly object lockObject = new object();

        public void Enqueue(T value)
        {
            lock(lockObject)
            {
                inQueue.Enqueue(value);
            }    
        }

        public IEnumerable<T> DequeueAll()
        {
            lock (lockObject)
            {
                var tmp = inQueue;
                inQueue = outQueue;
                outQueue = tmp;
            }

            while (outQueue.Count > 0)
            {
                yield return outQueue.Dequeue();
            }
        }

    }
}
