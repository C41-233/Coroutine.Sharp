using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Coroutines.Base;

namespace Coroutines
{

    public abstract class Waitable : IWaitable
    {

        private readonly SpinLock spinLock = new SpinLock();

        public WaitableStatus Status => status;

        private volatile WaitableStatus status;

        private readonly int id;

        protected Waitable()
        {
            id = IdGenerator.Next();
            status = WaitableStatus.Running;
        }

        private List<Action> successCallbacks = new List<Action>(1);

        public IWaitable Then(Action callback)
        {
            Assert.NotNull(callback, nameof(callback));

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

        private List<Action<Exception>> catchCallbacks = new List<Action<Exception>>(1);

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
                status = WaitableStatus.Error;
            }

            foreach (var callback in catchCallbacks)
            {
                callback(e);
            }
            Dispose();
        }

        public IWaitable Catch(Action<Exception> callback)
        {
            Assert.NotNull(callback, nameof(callback));

            var call = false;
            using (spinLock.Hold())
            {
                switch (status)
                {
                    case WaitableStatus.Abort:
                    case WaitableStatus.Error:
                        call = true;
                        break;
                    case WaitableStatus.Running:
                        catchCallbacks.Add(callback);
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
                status = WaitableStatus.Abort;
            }

            OnAbort(recursive);

            foreach (var callback in catchCallbacks)
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
            catchCallbacks = null;
        }


        public sealed override int GetHashCode()
        {
            return id;
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
                    ExceptionDispatchInfo.Capture(Exception).Throw();
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

        public IWaitable<T> Then(Action<T> callback)
        {
            Assert.NotNull(callback, nameof(callback));

            Then(() => callback(result));
            return this;
        }
    }

}
