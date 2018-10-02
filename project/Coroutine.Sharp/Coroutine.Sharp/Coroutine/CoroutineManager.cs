using System;
using System.Collections.Generic;
using Coroutine.Base;

namespace Coroutine
{

    public class CoroutineManager
    {

        public Coroutine StartCoroutine(IEnumerable<Waitable> co)
        {
            var coroutine = new Coroutine(this, co.GetEnumerator());
            return coroutine;
        }

        private readonly SwapQueue<Action> actions = new SwapQueue<Action>();

        public void OneLoop()
        {
            foreach (var action in actions.DequeueAll())
            {
                action();
            }
        }

        internal void Enqueue(Action callback)
        {
            actions.Enqueue(callback);
        }
    }

}
