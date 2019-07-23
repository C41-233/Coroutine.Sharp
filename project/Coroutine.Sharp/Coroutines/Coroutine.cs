﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Coroutines
{
    public sealed class Coroutine : IWaitable
    {

        public static IWaitable<T> Complete<T>(T value)
        {
            return new CompleteWaitable<T>(value);
        }

        private readonly CoroutineManager coroutineManager;
        private IEnumerator enumerator;
        private readonly BubbleExceptionApproach approach;

        private IWaitable waitable;

        internal Coroutine(CoroutineManager coroutineManager, IEnumerator co, BubbleExceptionApproach approach)
        {
            this.coroutineManager = coroutineManager;
            this.approach = approach;
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

            waitable = null;
            if (!moveNext)
            {
                Success();
                return;
            }

            var current = enumerator.Current;
            switch (current)
            {
                case null:
                    coroutineManager.Enqueue(NextStep);
                    break;
                case ICompleteWaitable _:
                    Success();
                    break;
                case IWaitableEnumerable waitableEnumerable:
                    waitableEnumerable.Bind(coroutineManager);
                    Dispatch(waitableEnumerable);
                    break;
                case IEnumerable enumerable:
                    Dispatch(coroutineManager.StartCoroutine(enumerable));
                    break;
                default:
                    Dispatch((IWaitable)current);
                    break;
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

        private void Dispose()
        {
            waitable = null;

            if (enumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
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

        private void Fail(Exception e)
        {
            if (Status != WaitableStatus.Running)
            {
                return;
            }

            Exception = e ?? new WaitableAbortException();
            Status = WaitableStatus.Fail;

            var localFailCallbacks = failCallbacks;
            Dispose();

            foreach (var callback in localFailCallbacks)
            {
                callback(Exception);
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
            Exception = null;

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

}
