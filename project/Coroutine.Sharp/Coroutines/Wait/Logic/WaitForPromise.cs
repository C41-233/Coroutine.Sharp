using System;

namespace Coroutines
{
    internal class WaitForPromise : WaitableTask
    {

        public WaitForPromise(Action<Action, Action<Exception>> promise)
        {
            promise(Success, Fail);
        }

    }
}
