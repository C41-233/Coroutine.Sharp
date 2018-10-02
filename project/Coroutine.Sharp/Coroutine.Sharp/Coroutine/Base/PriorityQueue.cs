using System;
using System.Collections.Generic;

namespace Coroutine.Base
{

    internal class PriorityQueue<T>
    {

        private const int DefaultSize = 16;

        private readonly Comparer<T> comparer;

        private T[] buffer;

        public int Count { get; private set; }

        public PriorityQueue() : this(Comparer<T>.Default)
        {
        }

        public PriorityQueue(Comparer<T> comparer)
        {
            this.comparer = comparer;
            buffer = new T[DefaultSize];
        }

        public void Enqueue(T value)
        {
            if (Count + 1 >= buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }
            buffer[++Count] = value;
            ShifUp(Count);
        }

        public T Top => buffer[1];

        public T Dequeue()
        {
            var first = buffer[1];
            buffer[1] = buffer[Count--];
            ShiftDown(1);
            return first;
        }

        private void ShifUp(int hole)
        {
            var value = buffer[hole];
            while (hole > 1 && Less(value, buffer[hole / 2]))
            {
                buffer[hole] = buffer[hole / 2];
                hole /= 2;
            }
            buffer[hole] = value;
        }

        private void ShiftDown(int hole)
        {
            var tmp = buffer[hole];
            while (hole * 2 <= Count)
            {
                var child = hole * 2;
                if (child != Count && Less(buffer[child + 1], buffer[child]))
                {
                    child++;
                }
                if (Less(buffer[child], tmp))
                {
                    buffer[hole] = buffer[child];
                }
                else
                {
                    break;
                }
                hole = child;
            }
            buffer[hole] = tmp;
        }

        private bool Less(T a, T b) => comparer.Compare(a, b) < 0;

    }

}
