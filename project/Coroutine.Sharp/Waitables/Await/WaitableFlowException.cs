using System;

namespace Coroutines.Waitables.Await
{
    internal sealed class WaitableFlowException : Exception
    {

        public WaitableFlowException(string message) : base(message)
        {
        }

    }
}
