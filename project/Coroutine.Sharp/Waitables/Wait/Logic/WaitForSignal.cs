using System;
using Coroutines.Base;
using Coroutines.Signals;

namespace Coroutines
{
    internal sealed class WaitForSignal<T> : WaitableTask<T>
    {

        private readonly SignalHandler handler;
        private readonly Predicate<T> predicate;

        public WaitForSignal(SignalManager.Container container, Predicate<T> predicate)
        {
            Assert.NotNull(container, nameof(container));

            handler = container.OnSignal<T>(Callback);
            this.predicate = predicate;
        }

        private void Callback(T signal)
        {
            if (predicate == null || predicate(signal))
            {
                handler.Dispose();
                Success(signal);
            }
        }
    }
}
