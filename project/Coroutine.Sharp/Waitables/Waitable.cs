using System;
using System.Collections.Generic;
using Coroutines.Base;

namespace Coroutines.Waitables
{

    public abstract class Waitable : IWaitable
    {
        public WaitableStatus Status => status;

        public Exception Exception { get; private set; }

        private List<Action> successCallbacks = new List<Action>(2);
        private List<Action<Exception>> failCallbacks = new List<Action<Exception>>(2);

        private readonly SpinLock spin = new SpinLock();
        private volatile WaitableStatus status = WaitableStatus.Running;

        private readonly int id;

        protected Waitable()
        {
            id = IdGenerator.Next();
        }

        public IWaitable Then(Action callback)
        {
            Assert.NotNull(callback, nameof(callback));

            var call = false;
            using (spin.Hold())
            {
                switch (status)
                {
                    case WaitableStatus.Running:
                        successCallbacks.Add(callback);
                        break;
                    case WaitableStatus.Success:
                        call = true;
                        break;
                }
            }

            if (call)
            {
                callback();
            }

            return this;
        }

        public IWaitable Catch(Action<Exception> callback)
        {
            Assert.NotNull(callback, nameof(callback));

            var call = false;
            using (spin.Hold())
            {
                switch (status)
                {
                    case WaitableStatus.Running:
                        failCallbacks.Add(callback);
                        break;
                    case WaitableStatus.Abort:
                    case WaitableStatus.Error:
                        call = true;
                        break;
                }
            }

            if (call)
            {
                callback(Exception);
            }

            return this;
        }

        private protected void Success()
        {
            using (spin.Hold())
            {
                if (status != WaitableStatus.Running)
                {
                    return;
                }

                status = WaitableStatus.Success;
            }

            var actions = successCallbacks;
            Dispose();

            foreach (var callback in actions)
            {
                callback();
            }
        }

        private protected void Fail(Exception e)
        {
            using (spin.Hold())
            {
                if (status != WaitableStatus.Running)
                {
                    return;
                }

                Exception = e;
                status = WaitableStatus.Error;
            }

            var actions = failCallbacks;
            Dispose();

            foreach (var callback in actions)
            {
                callback(e);
            }
        }

        public void Abort()
        {
            using (spin.Hold())
            {
                if (status != WaitableStatus.Running)
                {
                    return;
                }

                Exception = null;
                status = WaitableStatus.Abort;
            }

            OnAbort();

            var actions = failCallbacks;
            Dispose();
            foreach (var callback in actions)
            {
                callback(null);
            }
        }

        protected virtual void OnAbort()
        {
        }

        private void Dispose()
        {
            using (spin.Hold())
            {
                successCallbacks = null;
                failCallbacks = null;
            }
        }

        public override int GetHashCode()
        {
            return id;
        }
    }

}
