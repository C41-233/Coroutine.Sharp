﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coroutines.Base;

namespace Coroutines
{

    internal sealed class Coroutine : IThreadSafeWaitable
    {

        public static IWaitable<T> Complete<T>(T value)
        {
            return new CompleteWaitable<T>(value);
        }

        private readonly CoroutineManager.Container container;
        private CoroutineManager manager => container.CoroutineManager;
        private IEnumerator enumerator;
        private readonly BubbleExceptionApproach approach;

        private IWaitable waitable;

        private readonly int id;

        internal Coroutine(CoroutineManager.Container container, IEnumerator co, BubbleExceptionApproach approach)
        {
            this.container = container;
            this.approach = approach;
            id = IdGenerator.Next();
            enumerator = co;
            //下一帧执行
            Enqueue(NextStep, false);
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
                    Enqueue(NextStep, false);
                    break;
                case IEnumerable enumerable:
                    Dispatch(container.StartCoroutine(enumerable));
                    break;
                case Task task:
                    Dispatch(new WaitForTask(task));
                    break;
                default:
                    Dispatch((IWaitable)current);
                    break;
            }
        }

        private void Dispatch(IWaitable waitable)
        {
            this.waitable = waitable;
            if (waitable is IBindCoroutineWaitable bindCoroutineWaitable)
            {
                bindCoroutineWaitable.Bind(container);
            }

            NextThen();
            NextCatch();
        }

        private void NextThen()
        {
            //等待的事件成功，继续下一步
            waitable.Then(() =>
            {
                var fastCall = waitable is IThreadSafeWaitable;
                var complete = waitable is ICompleteCoroutineWaitable;
                waitable = null;

                if (complete)
                {
                    Enqueue(Success, fastCall);
                }
                else
                {
                    Enqueue(NextStep, fastCall);
                }
            });

        }

        private void NextCatch()
        {
            waitable.Catch(e =>
            {
                var fastCall = waitable is IThreadSafeWaitable;
                waitable = null;

                switch (approach)
                {
                    case BubbleExceptionApproach.Abort:
                        Enqueue(() => Abort(false), fastCall);
                        break;
                    case BubbleExceptionApproach.Throw:
                        Enqueue(() => Fail(e), fastCall);
                        break;
                    default:
                        if (waitable is ICompleteCoroutineWaitable)
                        {
                            Enqueue(() => Fail(e), fastCall);
                        }
                        else
                        {
                            //等待的事件失败，继续下一步，由调用者处理异常，coroutine本身未失败
                            Enqueue(NextStep, fastCall);
                        }
                        break;
                }
            });
        }

        private void Enqueue(Action action, bool fastCall)
        {
            if (fastCall)
            {
                action();
            }
            else
            {
                manager.Enqueue(action);
            }
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

        private List<Action> successCallbacks = new List<Action>(2);
        private List<Action<Exception>> failCallbacks = new List<Action<Exception>>(2);

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

        public IWaitable Then(Action callback)
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
            Status = WaitableStatus.Error;

            var localFailCallbacks = failCallbacks;
            Dispose();

            foreach (var callback in localFailCallbacks)
            {
                callback(Exception);
            }
        }

        public IWaitable Catch(Action<Exception> callback)
        {
            if (callback == null)
            {
                return this;
            }
            switch (Status)
            {
                case WaitableStatus.Error:
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

            Status = WaitableStatus.Abort;
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

        public override int GetHashCode()
        {
            return id;
        }
    }

}
