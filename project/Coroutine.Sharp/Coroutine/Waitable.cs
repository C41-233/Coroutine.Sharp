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

        IWaitable IWaitable.OnSuccess(Action callback)
        {
            if (callback == null)
            {
                return this;
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
            return this;
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

        IWaitable IWaitable.OnFail(Action<Exception> callback)
        {
            if (callback == null)
            {
                return this;
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
            return this;
        }

        void IWaitable.Abort()
        {
            if (status != WaitableStatus.Running)
            {
                return;
            }

            Exception = new WaitableAbortException();
            status = WaitableStatus.Fail;

            OnAbort();

            foreach (var callback in failCallbacks)
            {
                callback(Exception);
            }
            Dispose();
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

        public T Result
        {
            get
            {
                if (Exception != null)
                {
                    throw Exception;
                }
                return result;
            }
        }

        private T result;

        protected void Success(T result)
        {
            this.result = result;
            DoSuccess();
        }

        IWaitable<T> IWaitable<T>.OnSuccess(Action<T> callback)
        {
            if (callback == null)
            {
                return this;
            }
            ((IWaitable) this).OnSuccess(() => callback(result));
            return this;
        }
    }

    public abstract class WaitableTask<T1, T2> : Waitable, IWaitable<T1, T2>
    {

        public T1 Result1
        {
            get
            {
                if (Exception != null)
                {
                    throw Exception;
                }
                return result1;
            }
        }

        public T2 Result2
        {
            get
            {
                if (Exception != null)
                {
                    throw Exception;
                }
                return result2;
            }
        }

        private T1 result1;
        private T2 result2;

        protected void Success(T1 result1, T2 result2)
        {
            this.result1 = result1;
            this.result2 = result2;
            DoSuccess();
        }

        IWaitable<T1, T2> IWaitable<T1, T2>.OnSuccess(Action<T1, T2> callback)
        {
            if (callback == null)
            {
                return this;
            }
            (this as IWaitable).OnSuccess(() => callback(result1, result2));
            return this;
        }
    }
}
