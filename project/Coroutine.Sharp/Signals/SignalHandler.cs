using System;

namespace Coroutines.Signals
{
    public abstract class SignalHandler
    {
        public SignalManager.Container SignalContainer { get; }
        public Type SignalType { get; }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 释放监听函数，自释放后触发signal不再会被调用
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        internal virtual void Dispose(bool prop)
        {
            IsDisposed = true;
        }

        internal SignalHandler(SignalManager.Container container, Type type)
        {
            SignalContainer = container;
            SignalType = type;
        }

    }

    public sealed class SignalHandler<TSignal> : SignalHandler
    {
        public Action<TSignal> Delegate { get; private set; }

        internal SignalHandler(SignalManager.Container container, Action<TSignal> callback)
            : base(container, typeof(TSignal))
        {
            Delegate = callback;
        }

        /// <inheritdoc />
        internal override void Dispose(bool prop)
        {
            base.Dispose(prop);
            Delegate = null;
            if (prop)
            {
                SignalContainer.RemoveHandler(this);
            }
        }

    }

}