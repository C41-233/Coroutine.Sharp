using System;

namespace Coroutines
{

    internal sealed class WaitableFlowException : Exception
    {

        public WaitableFlowException(string message) : base(message)
        {
        }


    }

    /// <inheritdoc />
    /// <summary>
    /// 正在等待的IWaitable发生Abort，当前IWaitable自身中断
    /// </summary>
    public sealed class WaitableAbortException : Exception
    {

        internal WaitableAbortException() : base("Wait abort")
        {
        }

    }

}
