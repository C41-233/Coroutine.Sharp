using System;

namespace Coroutine
{

    public sealed class WaitableFlowException : Exception
    {

        public WaitableFlowException(Exception e) : base(null, e)
        {
        }

    }

    public sealed class WaitableAbortException : Exception
    {
    }
}
