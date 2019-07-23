using System;

namespace Coroutines
{
    public static class WaitableExtends
    {

        public static IWaitable Co(this IWaitable self, out IWaitable waitable)
        {
            waitable = self;
            return self;
        }

        public static IWaitable<T> Co<T>(this IWaitable<T> self, out WaitableValue<T> value)
        {
            value = new WaitableValue<T>(self);
            return self;
        }

        public static bool IsRunning(this IWaitable self)
        {
            return self.Status == WaitableStatus.Running;
        }

        public static bool IsSuccess(this IWaitable self)
        {
            return self.Status == WaitableStatus.Success;
        }

        public static bool IsFail(this IWaitable self)
        {
            return self.Status == WaitableStatus.Fail;
        }

        public static bool IsAbort(this IWaitable self)
        {
            return self.Status == WaitableStatus.Fail && self.Exception == null;
        }

        public static bool IsFinish(this IWaitable self)
        {
            return self.Status != WaitableStatus.Running;
        }

        public static bool IsError(this IWaitable self)
        {
            return self.Status == WaitableStatus.Fail && self.Exception != null;
        }

        public static IWaitable OnFail(this IWaitable self, Action callback)
        {
            if (callback == null)
            {
                return self;
            }
            return self.OnFail(e => callback());
        }

    }

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

        public IWaitable OnSuccess(Action callback)
        {
            return waitable.OnSuccess(callback);
        }

        public IWaitable OnFail(Action<Exception> callback)
        {
            return waitable.OnFail(callback);
        }

        public void Abort(bool recursive = true)
        {
            waitable.Abort(recursive);
        }

        public IWaitable<T> OnSuccess(Action<T> callback)
        {
            return waitable.OnSuccess(callback);
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