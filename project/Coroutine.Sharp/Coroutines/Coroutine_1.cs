using System;
using System.Collections.Generic;

namespace Coroutines
{

    public sealed class Coroutine<T> : IWaitable<T>
    {

        public static IWaitable<T> Complete(T value)
        {
            return new CoroutineResult<T>(value);
        }

        private CoroutineManager coroutineManager;
        private IEnumerator<IWaitable> enumerator;
        private BubbleExceptionApproach approach;

        private IWaitable waitable;

        public T R
        {
            get
            {
                if (Exception != null)
                {
                    throw new WaitableFlowException(Exception);
                }

                return r;
            }
        }

        private T r;

        internal Coroutine(IEnumerable<IWaitable> co)
        {
            enumerator = co.GetEnumerator();
        }

        internal void Start(CoroutineManager coroutineManager, BubbleExceptionApproach approach)
        {
            this.approach = approach;
            this.coroutineManager = coroutineManager;
            NextStep();
        }

        private void NextStep()
        {
            bool moveNext;
            try
            {
                moveNext = enumerator.MoveNext();
            }
            catch (Exception e)
            {
                //coroutine本身抛出异常，当前coroutine失败
                Fail(e);
                return;
            }

            if (!moveNext)
            {
                Success(default);
                return;
            }

            var current = enumerator.Current;

            if (current is CoroutineResult<T> result)
            {
                Success(result.R);
                return;
            }

            Dispatch(current);
        }

        private void Dispatch(IWaitable waitable)
        {
            this.waitable = waitable;

            if (waitable != null)
            {
                //等待的事件成功，继续下一步
                waitable.OnSuccess(() =>
                {
                    coroutineManager.Enqueue(NextStep);
                    this.waitable = null;
                });

                waitable.OnFail(e =>
                {
                    switch (approach)
                    {
                        case BubbleExceptionApproach.Abort:
                            coroutineManager.Enqueue(() => Abort(false));
                            break;
                        case BubbleExceptionApproach.Throw:
                            coroutineManager.Enqueue(() => Fail(e));
                            break;
                        default:
                            //等待的事件失败，继续下一步，由调用者处理异常，coroutine本身未失败
                            coroutineManager.Enqueue(NextStep);
                            break;
                    }

                    this.waitable = null;
                });
            }
            else
            {
                coroutineManager.Enqueue(NextStep);
            }
        }

        private void Dispose()
        {
            waitable = null;

            enumerator.Dispose();
            enumerator = null;

            successCallbacks = null;
            failCallbacks = null;
        }

        public WaitableStatus Status { get; private set; }
        public Exception Exception { get; private set; }

        private List<Action<T>> successCallbacks = new List<Action<T>>(1);
        private List<Action<Exception>> failCallbacks = new List<Action<Exception>>(1);

        private void Success(T t)
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            r = t;
            Status = WaitableStatus.Success;

            var localSuccessCallbacks = successCallbacks;
            Dispose();

            foreach (var callback in localSuccessCallbacks)
            {
                callback(t);
            }
        }

        public IWaitable OnSuccess(Action callback)
        {
            if (callback == null)
            {
                return this;
            }
            OnSuccess(t => callback());
            return this;
        }

        public IWaitable<T> OnSuccess(Action<T> callback)
        {
            if (callback == null)
            {
                return this;
            }

            switch (Status)
            {
                case WaitableStatus.Success:
                    callback(R);
                    break;
                case WaitableStatus.Running:
                    successCallbacks.Add(callback);
                    break;
            }
            return this;
        }

        private void Fail(Exception e)
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            Exception = e;
            Status = WaitableStatus.Fail;

            var localFailCallbacks = failCallbacks;
            Dispose();

            if (localFailCallbacks.Count > 0)
            {
                foreach (var callback in localFailCallbacks)
                {
                    callback(e);
                }
            }
            else
            {
                throw e is WaitableFlowException ? e : new WaitableFlowException(e);
            }
        }

        public IWaitable OnFail(Action<Exception> callback)
        {
            if (callback == null)
            {
                return this;
            }
            switch (Status)
            {
                case WaitableStatus.Fail:
                    callback(Exception);
                    break;
                case WaitableStatus.Running:
                    failCallbacks.Add(callback);
                    break;
            }
            return this;
        }

        public void Abort(bool recursive = true)
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            Status = WaitableStatus.Fail;
            Exception = new WaitableAbortException();

            if (recursive)
            {
                waitable?.Abort();
            }

            var localFailCallbacks = failCallbacks;
            Dispose();

            foreach (var callback in localFailCallbacks)
            {
                callback(Exception);
            }
        }
    }

    internal sealed class CoroutineResult<T> : IWaitable<T>
    {

        public T R { get; }

        WaitableStatus IWaitable.Status => WaitableStatus.Success;

        Exception IWaitable.Exception => null;

        public CoroutineResult(T r)
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
