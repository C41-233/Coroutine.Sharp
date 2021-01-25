using System;

namespace Coroutines
{

    /// <summary>
    /// 固定成功值的Waitable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class CompleteWaitable<T> : ICompleteCoroutineWaitable<T>
    {

        public T R { get; }

        WaitableStatus IWaitable.Status => WaitableStatus.Success;

        Exception IWaitable.Exception => null;

        public CompleteWaitable(T r)
        {
            R = r;
        }

        IWaitable IWaitable.Then(Action callback)
        {
            callback?.Invoke();
            return this;
        }

        public IWaitable<T> Then(Action<T> callback)
        {
            callback?.Invoke(R);
            return this;
        }

        public IWaitable Catch(Action<Exception> callback)
        {
            return this;
        }

        public void Abort(bool recursive)
        {
        }

    }
}