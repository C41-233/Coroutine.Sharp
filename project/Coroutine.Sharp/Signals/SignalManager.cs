using System;
using System.Collections.Generic;

namespace Coroutines.Signals
{
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
