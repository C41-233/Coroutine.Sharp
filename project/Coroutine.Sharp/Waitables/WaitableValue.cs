using System;

namespace Coroutines
{
    public struct WaitableValue<T> : IWaitable<T>
    {

        private readonly IWaitable<T> waitable;
        public T R => waitable.R;

        internal WaitableValue(IWaitable<T> waitable)
        {
            this.waitable = waitable;
        }

        public static implicit operator T(WaitableValue<T> self)
        {
            return self.R;
        }

        public WaitableStatus Status => waitable.Status;
        public Exception Exception => waitable.Exception;

        public IWaitable Then(Action callback)
        {
            return waitable.Then(callback);
        }

        public IWaitable Catch(Action<Exception> callback)
        {
            return waitable.Catch(callback);
        }

        public void Abort(bool recursive = true)
        {
            waitable.Abort(recursive);
        }

        public IWaitable<T> Then(Action<T> callback)
        {
            return waitable.Then(callback);
        }

        public override string ToString()
        {
            if (waitable.Status == WaitableStatus.Success)
            {
                return waitable.R?.ToString() ?? "";
            }

            return "";
        }
    }
}