using System;
using System.Collections.Generic;

namespace Coroutine
{
    public abstract class Waitable : IWaitable
    {

        WaitableStatus IWaitable.Status => status;

        private volatile WaitableStatus status;

        protected Waitable()
        {
            status = WaitableStatus.Running;
        }

        private List<Action> successCallbacks = new List<Action>(1);

        void IWaitable.OnSuccess(Action callback)
        {
            if (callback == null)
            {
                return;
            }
            switch (status)
            {
                case WaitableStatus.Success:
                    callback();
                    break;
                case WaitableStatus.Running:
                    successCallbacks.Add(callback);
                    break;
            }
        }

        internal void DoSuccess()
        {
            if (status != WaitableStatus.Running)
            {
                return;
            }
            
            status = WaitableStatus.Success;
            foreach (var callback in successCallbacks)
            {
                callback();
            }
            Dispose();
        }

        private List<Action<Exception>> failCallbacks = new List<Action<Exception>>(1);

        public Exception Exception { get; private set; }

        protected void Fail(Exception e)
        {
            if (status != WaitableStatus.Running)
            {
                return;
            }

            Exception = e;
            status = WaitableStatus.Fail;
            foreach (var callback in failCallbacks)
            {
                callback(e);
            }
            Dispose();
        }

        void IWaitable.OnFail(Action callback)
        {
            if (callback == null)
            {
                return;
            }
            (this as IWaitable).OnFail(e => callback());
        }

        void IWaitable.OnFail(Action<Exception> callback)
        {
            if (callback == null)
            {
                return;
            }
            switch (status)
            {
                case WaitableStatus.Fail:
                    callback(Exception);
                    break;
                case WaitableStatus.Running:
                    failCallbacks.Add(callback);
                    break;
            }
        }

        protected virtual void OnAbort()
        {
        }

        private void Dispose()
        {
            successCallbacks = null;
            failCallbacks = null;
        }

    }

    public abstract class WaitableTask : Waitable
    {

        protected void Success()
        {
            DoSuccess();
        }

    }

    public abstract class WaitableTask<T> : Waitable, IWaitable<T>
    {

        protected void Success(T result)
        {
            DoSuccess();
        }

        void IWaitable<T>.OnSuccess(Action<T> callback)
        {
        }
    }

}
