using System;

namespace Coroutines.Signals
{
    public abstract class SignalHandler
    {
        public SignalManager.Container SignalContainer { get; }
        public Type SignalType { get; }

        public bool IsDisposed { get; private set; }

        public abstract int Remain { get; }

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

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public override int Remain => remain;

        private int remain;

        internal SignalHandler(SignalManager.Container container, Action<TSignal> callback, int times)
            : base(container, typeof(TSignal))
        {
            Delegate = callback;
            remain = times;
        }

        /// <inheritdoc />
        internal override void Dispose(bool prop)
        {
            base.Dispose(prop);
            Delegate = null;
            remain = 0;
            if (prop)
            {
                SignalContainer.RemoveHandler(this);
            }
        }

        internal void Invoke(ref TSignal signal)
        {
            Delegate(signal);
            if (--remain == 0)
            {
                Dispose(false);
            }
        }

    }

}