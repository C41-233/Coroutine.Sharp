using System;
using System.Collections.Generic;
using Coroutine.Base;

namespace Coroutine
{
    public class CoroutineManager
    {

        public Coroutine StartCoroutine(IEnumerable<IWaitable> co)
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

        public Action<Coroutine, Exception> OnUnhandledException { private get; set; } = DefaultUnhandledException;

        private static void DefaultUnhandledException(Coroutine c, Exception e)
        {
            Console.Error.WriteLine(e);
        }

        internal void RaiseUnhandledException(Coroutine coroutine, Exception exception)
        {
            OnUnhandledException?.Invoke(coroutine, exception);
        }
    }

}
