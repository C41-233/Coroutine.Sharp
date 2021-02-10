using System;

namespace Coroutines.Waitables.Await
{
    public sealed class WaitableFlowException : Exception
    {

        public WaitableFlowException(string message) : base(message)
        {
        }

        internal static WaitableFlowException NotAsyncMethod()
        {
            throw new WaitableFlowException("StartCoroutine only accept async method");
        }

        internal static WaitableFlowException AsyncCallDirectly()
        {
            throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine instead.");
        }

    }
}
