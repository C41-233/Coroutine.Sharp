using System;
using System.Collections.Generic;
using Coroutine.Base;

namespace Coroutine
{
    public class CoroutineManager
    {

        public Coroutine StartCoroutine(IEnumerable<IWaitable> co, BubbleException bubbleException = BubbleException.Ignore)
        {
            var coroutine = new Coroutine(this, co.GetEnumerator(), bubbleException);
            return coroutine;
        }

        private readonly SwapQueue<Action> actions = new SwapQueue<Action>();

        public void OneLoop()
        {
            foreach (var action in actions.DequeueAll())
            {
                action();
            }
        }

        internal void Enqueue(Action callback)
        {
            actions.Enqueue(callback);
        }

        public Action<Exception> OnUnhandledException { internal get; set; } = DefaultUnhandledException;

        private static void DefaultUnhandledException(Exception e)
        {
            Console.Error.WriteLine(e);
        }

    }

    /// <summary>
    /// 当前Coroutine正在等待的IWaitable失败时的处理方法
    /// </summary>
    public enum BubbleException
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
