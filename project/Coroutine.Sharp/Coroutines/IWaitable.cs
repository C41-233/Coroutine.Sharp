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

}