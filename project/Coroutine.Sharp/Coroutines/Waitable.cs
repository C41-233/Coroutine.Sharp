using System;
using System.Collections.Generic;
using Coroutines.Base;

namespace Coroutines
{

    public abstract class Waitable : IWaitable
    {

        private readonly SpinLock spinLock = new SpinLock();

        public WaitableStatus Status => status;

        private volatile WaitableStatus status;

        protected Waitable()
        {
            status = WaitableStatus.Running;
        }

        private List<Action> successCallbacks = new List<Action>(1);

        public IWaitable OnSuccess(Action callback)
        {
            if (callback == null)
            {
                return this;
            }

            var call = false;
            using (spinLock.Hold())
            {
                switch (status)
                {
                    case WaitableStatus.Success:
                        call = true;
                        break;
                    case WaitableStatus.Running:
                        successCallbacks.Add(callback);
                        break;
                }
            }

            if (call)
            {
                callback();
            }

            return this;
        }

        internal void DoSuccess()
        {
            using (spinLock.Hold())
            {
                if (status != WaitableStatus.Running)
                {
                    return;
                }

                status = WaitableStatus.Success;
            }

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
            using (spinLock.Hold())
            {
                if (status != WaitableStatus.Running)
                {
                    return;
                }

                Exception = e;
                status = WaitableStatus.Fail;
            }

            foreach (var callback in failCallbacks)
            {
                callback(e);
            }
            Dispose();
        }

        public IWaitable OnFail(Action<Exception> callback)
        {
            if (callback == null)
            {
                return this;
            }

            var call = false;
            using (spinLock.Hold())
            {
                switch (status)
                {
                    case WaitableStatus.Fail:
                        call = true;
                        break;
                    case WaitableStatus.Running:
                        failCallbacks.Add(callback);
                        break;
                }
            }

            if (call)
            {
                callback(Exception);
            }

            return this;
        }

        public void Abort(bool recursive = true)
        {
            using (spinLock.Hold())
            {
                if (status != WaitableStatus.Running)
                {
                    return;
                }

                Exception = null;
                status = WaitableStatus.Fail;
            }

            OnAbort(recursive);

            foreach (var callback in failCallbacks)
            {
                callback(Exception);
            }
            Dispose();
        }

        protected virtual void OnAbort(bool recursive)
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

        public T R
        {
            get
            {
                if (Exception != null)
                {
                    throw new WaitableFlowException(Exception);
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

        public IWaitable<T> OnSuccess(Action<T> callback)
        {
            if (callback == null)
            {
                return this;
            }
            OnSuccess(() => callback(result));
            return this;
        }
    }

}
