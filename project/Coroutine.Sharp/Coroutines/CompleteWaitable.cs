using System;

namespace Coroutines
{

    internal interface ICompleteWaitable
    {
    }

    /// <summary>
    /// 固定成功值的Waitable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class CompleteWaitable<T> : IWaitable<T>, ICompleteWaitable
    {

        public T R { get; }

        WaitableStatus IWaitable.Status => WaitableStatus.Success;

        Exception IWaitable.Exception => null;

        public CompleteWaitable(T r)
        {
            R = r;
        }

        IWaitable IWaitable.OnSuccess(Action callback)
        {
            callback?.Invoke();
            return this;
        }

        public IWaitable<T> OnSuccess(Action<T> callback)
        {
            callback?.Invoke(R);
            return this;
        }

        public IWaitable OnFail(Action<Exception> callback)
        {
            return this;
        }

        public void Abort(bool recursive)
        {
        }

    }
}