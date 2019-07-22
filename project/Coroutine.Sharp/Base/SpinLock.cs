using System;
using System.Threading;

namespace Coroutine.Base
{
    internal class SpinLock
    {

        private int value;

        internal void Enter()
        {
            while (Interlocked.CompareExchange(ref value, 1, 0) == 1)
            {
            }
        }

        internal void Exit()
        {
            Interlocked.Exchange(ref value, 0);
        }

        public SpinLockHolder Hold()
        {
            return new SpinLockHolder(this);
        }

    }

    internal struct SpinLockHolder : IDisposable
    {

        private readonly SpinLock spinLock;

        public SpinLockHolder(SpinLock spinLock)
        {
            this.spinLock = spinLock;
            this.spinLock.Enter();
        }

        void IDisposable.Dispose()
        {
            spinLock.Exit();
        }

    }
}
