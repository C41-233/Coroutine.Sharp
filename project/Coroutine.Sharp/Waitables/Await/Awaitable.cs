using System;
using Coroutines.Base;

namespace Coroutines.Await
{

    internal class Awaitable : WaitableTask
    {
        public CoroutineManager.Container CoroutineContainer { get; }

        private readonly DebugInfo debugInfo;

        public Awaitable(CoroutineManager.Container container, DebugInfo debugInfo)
        {
            CoroutineContainer = container;
            this.debugInfo = debugInfo;
        }

        public new void Success()
        {
            base.Success();
        }

        public new void Fail(Exception e)
        {
            base.Fail(e);
        }

        public override string ToString()
        {
            var name = debugInfo.Name ?? $"{nameof(Awaitable)}_{GetHashCode()}";
            return $"{name} | {debugInfo.Method} {debugInfo.File}:{debugInfo.Line}";
        }
    }

    internal class Awaitable<T> : WaitableTask<T>
    {
        public CoroutineManager.Container CoroutineContainer { get; }

        private readonly DebugInfo debugInfo;

        public Awaitable(CoroutineManager.Container container, DebugInfo debugInfo)
        {
            CoroutineContainer = container;
            this.debugInfo = debugInfo;
        }

        public new void Success(T value)
        {
            base.Success(value);
        }

        public new void Fail(Exception e)
        {
            base.Fail(e);
        }

        public override string ToString()
        {
            var name = debugInfo.Name ?? $"{nameof(Awaitable<T>)}_{GetHashCode()}";
            return $"{name} | {debugInfo.Method} {debugInfo.File}:{debugInfo.Line}";
        }
    }
}
