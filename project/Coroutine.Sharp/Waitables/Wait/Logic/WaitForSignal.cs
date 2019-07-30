using System;
using Coroutines.Base;
using Coroutines.Signals;

namespace Coroutines
{
    internal sealed class WaitForSignal<T> : WaitableTask<T>
    {

        public WaitForSignal(SignalManager.Container container)
        {
            Assert.NotNull(container, nameof(container));

            container.OnSignal<T>(Success, 1);
        }

        public WaitForSignal(SignalManager.Container container, Predicate<T> predicate)
        {
            Assert.NotNull(container, nameof(container));
            Assert.NotNull(predicate, nameof(predicate));

            container.OnSignal<T>(signal =>
            {
                if (predicate(signal))
                {
                    Success(signal);
                }
            }, 1);
        }

    }
}
