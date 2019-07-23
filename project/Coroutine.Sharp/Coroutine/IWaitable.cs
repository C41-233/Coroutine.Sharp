using System;

namespace Coroutine
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

        T Result { get; }

        IWaitable<T> OnSuccess(Action<T> callback);

    }

    public interface IWaitable<out T1, out T2> : IWaitable
    {
        T1 Result1 { get; }

        T2 Result2 { get; }

        IWaitable<T1, T2> OnSuccess(Action<T1, T2> callback);
    }

    public static class WaitableExtends
    {

        public static IWaitable Co(this IWaitable self, out IWaitable waitable)
        {
            waitable = self;
            return self;
        }

        public static bool IsAbort(this IWaitable self)
        {
            return self.Status == WaitableStatus.Fail && self.Exception is WaitableAbortException;
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