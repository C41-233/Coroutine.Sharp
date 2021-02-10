using System;

namespace Coroutines.Waitables
{
    public abstract class WaitableTask : Waitable
    {

        protected new void Success()
        {
            base.Success();
        }

        protected new void Fail(Exception e)
        {
            base.Fail(e);
        }

    }
}
