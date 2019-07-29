using System;
using System.Collections.Generic;

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

    public class SignalManager
    {

        private class SignalHandlers
        {
            public bool Delete;
            public readonly List<SignalHandler> List = new List<SignalHandler>();
        }

        private readonly Dictionary<Type, SignalHandlers> dictionary = new Dictionary<Type, SignalHandlers>();

        private int callStack;

        public void ReleaseSignal<TSignal>(TSignal signal)
        {
            if (!dictionary.TryGetValue(typeof(TSignal), out var handlers))
            {
                return;
            }

            callStack++;
            var list = handlers.List;
            for (int i = 0, count = list.Count; i < count; i++)
            {
                var baseHandler = list[i];
                if (baseHandler.IsDisposed)
                {
                    handlers.Delete = true;
                    continue;
                }

                var handler = (SignalHandler<TSignal>) baseHandler;
                handler.Delegate(signal);
            }
            callStack--;

            if (callStack == 0 && handlers.Delete)
            {
                handlers.Delete = false;
                handlers.List.RemoveAll(handler => handler.IsDisposed);
            }
        }

        private void AddHandler(SignalHandler handler)
        {
            if (!dictionary.TryGetValue(handler.SignalType, out var handlers))
            {
                handlers = new SignalHandlers();
                dictionary.Add(handler.SignalType, handlers);
            }

            handlers.List.Add(handler);
        }

        private void RemoveHandler(SignalHandler handler)
        {
            if (!dictionary.TryGetValue(handler.SignalType, out var handlers))
            {
                return;
            }
            if (callStack > 0)
            {
                handlers.Delete = true;
                return;
            }

            handlers.List.Remove(handler);
        }

        public sealed class Container
        {

            private readonly SignalManager manager;
           
            private readonly List<SignalHandler> handlers = new List<SignalHandler>();

            internal Container(SignalManager manager)
            {
                this.manager = manager;
            }

            /// <summary>
            /// 注册一个signal监听函数，自注册后释放的signal会触发该监听函数
            /// </summary>
            /// <typeparam name="T">signal类型</typeparam>
            /// <param name="action">监听函数</param>
            /// <returns>handler</returns>
            public SignalHandler<T> OnSignal<T>(Action<T> action)
            {
                var handler = new SignalHandler<T>(this, action);
                handlers.Add(handler);
                manager.AddHandler(handler);
                return handler;
            }

            internal void RemoveHandler(SignalHandler handler)
            {
                handlers.Remove(handler);
                manager.RemoveHandler(handler);
            }

            public void ClearAllHandlers()
            {
                foreach (var handler in handlers)
                {
                    manager.RemoveHandler(handler);
                    handler.Dispose(false);
                }
                handlers.Clear();
            }

        }

    }

}
