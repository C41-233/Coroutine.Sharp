using System;
using System.Collections.Generic;

namespace Coroutine
{

    public abstract class Waitable
    {

        private CoroutineManager coroutineManager;

        internal void BindContext(CoroutineManager coroutineManager)
        {
            this.coroutineManager = coroutineManager;
        }

        private readonly List<Action> successCallbacks = new List<Action>(1);

        public void OnSuccess(Action callback)
        {
            successCallbacks.Add(callback);
        }

        internal void OnSuccess()
        {
            foreach (var callback in successCallbacks)
            {
                coroutineManager.Enqueue(callback);
            }
        }

        protected void Fail(Exception e)
        {
            
        }

        protected abstract void OnAbort();

    }

    public abstract class WaitableTask : Waitable
    {

        protected void Success()
        {
            OnSuccess();
        }

    }

    public abstract class WaitableTask<T> : Waitable
    {

        protected void Success(T result)
        {
            OnSuccess();
        }

    }

}
