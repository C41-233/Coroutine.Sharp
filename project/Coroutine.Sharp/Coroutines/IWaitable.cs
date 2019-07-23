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

}