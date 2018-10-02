using System;

namespace Coroutine
{
    public class Promise : WaitableTask
    {

        public Promise(Action<Action, Action<Exception>> promise)
        {
            promise(Success, Fail);
        }

        protected override void OnAbort()
        {
        }

    }
}
