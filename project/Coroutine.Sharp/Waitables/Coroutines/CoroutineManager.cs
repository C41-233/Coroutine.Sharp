using System;
using System.Collections;
using System.Collections.Generic;
using Coroutines.Await;
using Coroutines.Base;

namespace Coroutines
{
    public class CoroutineManager
    {

        public BubbleExceptionApproach DefaultBubbleExceptionApproach { get; set; } = BubbleExceptionApproach.Ignore;

        private readonly SwapQueue<Action> actions = new SwapQueue<Action>();

        public void OneLoop()
        {
            foreach (var action in actions.DequeueAll())
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        internal void Enqueue(Action callback)
        {
            actions.Enqueue(callback);
        }

        public class Container
        {

            public CoroutineManager CoroutineManager { get; }

            private readonly HashSet<IWaitable> waitables = new HashSet<IWaitable>();

            internal Container(CoroutineManager coroutineManager)
            {
                CoroutineManager = coroutineManager;
            }

            public Coroutine<T> StartCoroutine<T>(IEnumerable co, BubbleExceptionApproach bubbleExceptionApproach)
            {
                var coroutine = new Coroutine<T>(this, co, bubbleExceptionApproach);
                waitables.Add(coroutine);
                coroutine.Finally(() => waitables.Remove(coroutine));
                return coroutine;
            }

            public Coroutine<T> StartCoroutine<T>(IEnumerable co)
            {
                return StartCoroutine<T>(co, CoroutineManager.DefaultBubbleExceptionApproach);
            }

            public Coroutine StartCoroutine(IEnumerable co, BubbleExceptionApproach bubbleExceptionApproach)
            {
                var coroutine = new Coroutine(this, co.GetEnumerator(), bubbleExceptionApproach);
                waitables.Add(coroutine);
                coroutine.Finally(() => waitables.Remove(coroutine));
                return coroutine;
            }

            public Coroutine StartCoroutine(IEnumerable co)
            {
                return StartCoroutine(co, CoroutineManager.DefaultBubbleExceptionApproach);
            }

            public Coroutine StartCoroutine(Func<Coroutine> func)
            {
                CoroutineAwaitMethodBuilder.coroutineManager = CoroutineManager;
                return func();
            }

            public void Clear()
            {
                var list = new IWaitable[waitables.Count];
                waitables.CopyTo(list);
                waitables.Clear();
                foreach (var waitable in list)
                {
                    waitable.Abort();
                }
            }

        }

        public Container CreateContainer() => new Container(this);

    }

    /// <summary>
    /// 当前Coroutine正在等待的IWaitable失败时的处理方法
    /// </summary>
    public enum BubbleExceptionApproach
    {
        /// <summary>
        /// 不处理，由调用者主动处理
        /// </summary>
        Ignore,

        /// <summary>
        /// 中断当前的Coroutine
        /// </summary>
        Abort,

        /// <summary>
        /// 级联抛出异常交由异常处理
        /// </summary>
        Throw,
    }

}
