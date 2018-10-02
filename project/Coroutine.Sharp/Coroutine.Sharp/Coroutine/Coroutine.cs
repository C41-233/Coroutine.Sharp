using System;
using System.Collections.Generic;

namespace Coroutine
{
    public class Coroutine : IWaitable
    {

        private readonly CoroutineManager coroutineManager;
        private IEnumerator<IWaitable> enumerator;
        private IWaitable waitable;

        internal Coroutine(CoroutineManager coroutineManager, IEnumerator<IWaitable> co)
        {
            this.coroutineManager = coroutineManager;
            enumerator = co;

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

            if (moveNext)
            {
                Dispatch(enumerator.Current);
            }
            else
            {
                Success();
            }
        }

        private void Dispatch(IWaitable waitable)
        {
            this.waitable = waitable;

            //等待的事件成功，继续下一步
            waitable.OnSuccess(() =>
            {
                this.waitable = null;
                coroutineManager.Enqueue(NextStep);
            });

            //等待的事件失败，继续下一步，由调用者处理异常，coroutine本身未失败
            waitable.OnFail(e =>
            {
                this.waitable = null;
                coroutineManager.Enqueue(() =>
                {
                    coroutineManager.Enqueue(NextStep);
                });
            });
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

        private List<Action> successCallbacks = new List<Action>(1);
        private List<Action<Exception>> failCallbacks = new List<Action<Exception>>(1);

        private void Success()
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            Status = WaitableStatus.Success;
            foreach (var callback in successCallbacks)
            {
                callback();
            }
            Dispose();
        }

        public void OnSuccess(Action callback)
        {
            if (callback == null)
            {
                return;
            }

            switch (Status)
            {
                case WaitableStatus.Success:
                    callback();
                    break;
                case WaitableStatus.Running:
                    successCallbacks.Add(callback);
                    break;
            }
        }

        private void Fail(Exception e)
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            Exception = e;
            Status = WaitableStatus.Fail;

            if (failCallbacks.Count > 0)
            {
                foreach (var callback in failCallbacks)
                {
                    callback(e);
                }
            }
            else
            {
                coroutineManager.RaiseUnhandledException(this, e);
            }

            Dispose();
        }

        public void OnFail(Action callback)
        {
            if (callback == null)
            {
                return;
            }
            OnFail(e => callback());
        }

        public void OnFail(Action<Exception> callback)
        {
            if (callback == null)
            {
                return;
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
        }
    }

}
