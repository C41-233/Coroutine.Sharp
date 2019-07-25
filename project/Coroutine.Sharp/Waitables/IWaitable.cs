using System;
using System.Runtime.CompilerServices;
using Coroutines.Await;

namespace Coroutines
{

    [AsyncMethodBuilder(typeof(AwaitMethodBuilder))]
    public interface IWaitable
    {

        WaitableStatus Status { get; }

        Exception Exception { get; }

        IWaitable Then(Action callback);

        IWaitable Catch(Action<Exception> callback);

        void Abort(bool recursive = true);
    }

    public interface IWaitable<out T> : IWaitable
    {

        T R { get; }

        IWaitable<T> Then(Action<T> callback);

    }

}