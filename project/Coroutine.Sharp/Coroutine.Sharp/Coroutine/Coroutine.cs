using System;
using System.Collections.Generic;

namespace Coroutine
{
    public class Coroutine : WaitableTask
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

        protected override void OnDispose()
        {
            waitable = null;

            enumerator.Dispose();
            enumerator = null;
        }

    }

}
