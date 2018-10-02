using System;

namespace Coroutine
{
    public interface IWaitable
    {

        WaitableStatus Status { get; }

        Exception Exception { get; }

        void OnSuccess(Action callback);

        void OnFail(Action callback);

        void OnFail(Action<Exception> callback);

    }

    public interface IWaitable<out T> : IWaitable
    {

        void OnSuccess(Action<T> callback);

    }

}