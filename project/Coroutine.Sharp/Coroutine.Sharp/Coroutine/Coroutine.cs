﻿using System;
using System.Collections.Generic;
using Coroutine.Wait;

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
                coroutineManager.Enqueue(NextStep);
                this.waitable = null;
            });

            waitable.OnFail(e =>
            {
                if (this.waitable is BreakOnFailWaitable)
                {
                    coroutineManager.Enqueue(() => AbortFail(e));
                }
                else
                {
                    //等待的事件失败，继续下一步，由调用者处理异常，coroutine本身未失败
                    coroutineManager.Enqueue(NextStep);
                }
                this.waitable = null;
            });
        }

        private void Dispose()
        {
            waitable = null;

            enumerator.Dispose();
            enumerator = null;

            SuccessCallbacks = null;
            FailCallbacks = null;
        }

        public WaitableStatus Status { get; private set; }
        public Exception Exception { get; private set; }

        private List<Action> SuccessCallbacks = new List<Action>(1);
        private List<Action<Exception>> FailCallbacks = new List<Action<Exception>>(1);

        private void Success()
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            Status = WaitableStatus.Success;
            var successCallbacks = SuccessCallbacks;
            Dispose();

            foreach (var callback in successCallbacks)
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
                    SuccessCallbacks.Add(callback);
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

            var failCallbacks = FailCallbacks;
            Dispose();

            foreach (var callback in failCallbacks)
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

            var failCallbacks = FailCallbacks;
            Dispose();

            if (failCallbacks.Count > 0)
            {
                foreach (var callback in failCallbacks)
                {
                    callback(e);
                }
            }
            else
            {
                coroutineManager.OnUnhandledException?.Invoke(this, e);
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
                    FailCallbacks.Add(callback);
                    break;
            }
            return this;
        }

        public void Abort()
        {
            Status = WaitableStatus.Fail;
            Exception = new WaitableAbortException();

            waitable?.Abort();

            var failCallbacks = FailCallbacks;
            Dispose();

            foreach (var callback in failCallbacks)
            {
                callback(Exception);
            }
        }
    }

}
