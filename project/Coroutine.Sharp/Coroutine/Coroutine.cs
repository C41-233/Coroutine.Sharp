using System;
using System.Collections.Generic;
using Coroutine.Wait;

namespace Coroutine
{
    public sealed class Coroutine : IWaitable
    {

        private readonly CoroutineManager coroutineManager;
        private IEnumerator<IWaitable> enumerator;
        private readonly BubbleException bubbleException;

        private IWaitable waitable;

        internal Coroutine(CoroutineManager coroutineManager, IEnumerator<IWaitable> co, BubbleException bubbleException)
        {
            this.coroutineManager = coroutineManager;
            this.bubbleException = bubbleException;

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
                    switch (bubbleException)
                    {
                        case BubbleException.Abort:
                            coroutineManager.Enqueue(() => AbortFail(e));
                            break;
                        case BubbleException.Throw:
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

        private List<Action> successCallbacks = new List<Action>(1);
        private List<Action<Exception>> failCallbacks = new List<Action<Exception>>(1);

        private void Success()
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            Status = WaitableStatus.Success;
            var localSuccessCallbacks = successCallbacks;
            Dispose();

            foreach (var callback in localSuccessCallbacks)
            {
                callback();
            }
        }

        public IWaitable OnSuccess(Action callback)
        {
            if (callback == null)
            {
                return this;
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
            return this;
        }

        private void AbortFail(Exception e)
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            Exception = e;
            Status = WaitableStatus.Fail;

            var localFailCallbacks = failCallbacks;
            Dispose();

            foreach (var callback in localFailCallbacks)
            {
                callback(e);
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
                coroutineManager.OnUnhandledException?.Invoke(e);
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

        public void Abort()
        {
            Status = WaitableStatus.Fail;
            Exception = new WaitableAbortException();

            waitable?.Abort();

            var localFailCallbacks = failCallbacks;
            Dispose();

            foreach (var callback in localFailCallbacks)
            {
                callback(Exception);
            }
        }
    }

}
