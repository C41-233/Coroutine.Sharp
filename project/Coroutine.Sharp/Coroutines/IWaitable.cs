using System;

namespace Coroutines
{
    public interface IWaitable
    {

        WaitableStatus Status { get; }

        Exception Exception { get; }

        IWaitable OnSuccess(Action callback);

        IWaitable OnFail(Action<Exception> callback);

        void Abort(bool recursive = true);
    }

    public interface IWaitable<out T> : IWaitable
    {

        T R { get; }

        IWaitable<T> OnSuccess(Action<T> callback);

    }

    public interface IWaitable<out T1, out T2> : IWaitable
    {
        T1 R1 { get; }

        T2 R2 { get; }

        IWaitable<T1, T2> OnSuccess(Action<T1, T2> callback);
    }

    public struct WaitableValue<T>
    {

        private readonly IWaitable<T> waitable;

        public static implicit operator T(WaitableValue<T> waitable)
        {
            return waitable.waitable.R;
        }

        internal WaitableValue(IWaitable<T> waitable)
        {
            this.waitable = waitable;
        }

        public override string ToString()
        {
            return waitable.R?.ToString() ?? "";
        }
    }

    public static class WaitableExtends
    {

        public static IWaitable Co(this IWaitable self, out IWaitable waitable)
        {
            waitable = self;
            return self;
        }

        public static IWaitable<T> Co<T>(this IWaitable<T> self, out WaitableValue<T> waitable)
        {
            waitable = new WaitableValue<T>(self);
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

}