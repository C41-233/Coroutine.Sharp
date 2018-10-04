using System;

namespace Coroutine
{
    public interface IWaitable
    {

        WaitableStatus Status { get; }

        Exception Exception { get; }

        void OnSuccess(Action callback);


        void OnFail(Action<Exception> callback);

        void Abort();
    }

    public interface IWaitable<out T> : IWaitable
    {

        void OnSuccess(Action<T> callback);

    }

    public static class WaitableExtends
    {

        public static void OnFail(this IWaitable self, Action callback)
        {
            if (callback == null)
            {
                return;
            }
            self.OnFail(e => callback());
        }

    }

}