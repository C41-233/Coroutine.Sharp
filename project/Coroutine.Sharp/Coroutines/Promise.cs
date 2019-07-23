using System;

namespace Coroutines
{
    public class Promise : WaitableTask
    {

        public Promise(Action<Action, Action<Exception>> promise)
        {
            promise(Success, Fail);
        }

    }
}
