using System;
using System.Collections.Generic;
using Coroutine.Base;

namespace Coroutine
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

        public void Abort()
        {
            using (spinLock.Hold())
            {
                if (status != WaitableStatus.Running)
                {
                    return;
                }

                Exception = new WaitableAbortException();
                status = WaitableStatus.Fail;
            }

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

        public IWaitable<T1, T2> OnSuccess(Action<T1, T2> callback)
        {
            if (callback == null)
            {
                return this;
            }
            OnSuccess(() => callback(result1, result2));
            return this;
        }
    }
}
