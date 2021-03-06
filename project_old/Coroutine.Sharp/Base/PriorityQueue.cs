﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Coroutines.Base
{

    internal class PriorityQueue<T> : IEnumerable<T>
    {

        private const int DefaultSize = 16;

        private readonly Comparison<T> comparison;

        private T[] buffer;

        public int Count { get; private set; }

        public PriorityQueue(Comparison<T> comparison)
        {
            this.comparison = comparison;
            buffer = new T[DefaultSize];
        }

        public void Enqueue(T value)
        {
            if (Count + 1 >= buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }
            buffer[++Count] = value;
            ShiftUp(Count);
        }

        public T Top => buffer[1];

        public T Dequeue()
        {
            var first = buffer[1];
            buffer[1] = buffer[Count--];
            ShiftDown(1);
            return first;
        }

        private void ShiftUp(int hole)
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

        private bool Less(T a, T b) => comparison(a, b) < 0;

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 1; i <= Count; i++)
            {
                yield return buffer[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            for (var i = 1; i <= Count; i++)
            {
                buffer[i] = default;
            }

            Count = 0;
        }

    }

}
