using System;
using System.Collections.Generic;
using Coroutines.Base;

namespace Coroutines
{
    public class CoroutineManager
    {

        public BubbleExceptionApproach DefaultBubbleExceptionApproach { get; set; } = BubbleExceptionApproach.Ignore;

        public Coroutine<T> StartCoroutine<T>(IEnumerable<IWaitable> co, BubbleExceptionApproach bubbleExceptionApproach)
        {
            var coroutine = new Coroutine<T>(co);
            try
            {
                coroutine.Start(this, bubbleExceptionApproach);
            }
            catch (Exception e)
            {
                UnHandleException(e);
            }
            return coroutine;
        }

        public Coroutine<T> StartCoroutine<T>(IEnumerable<IWaitable> co)
        {
            return StartCoroutine<T>(co, DefaultBubbleExceptionApproach);
        }

        public Coroutine StartCoroutine(IEnumerable<IWaitable> co)
        {
            return StartCoroutine(co, DefaultBubbleExceptionApproach);
        }

        public Coroutine StartCoroutine(IEnumerable<IWaitable> co, BubbleExceptionApproach bubbleExceptionApproach)
        {
            var coroutine = new Coroutine(co.GetEnumerator());
            try
            {
                coroutine.Start(this, bubbleExceptionApproach);
            }
            catch (Exception e)
            {
                UnHandleException(e);
            }
            return coroutine;
        }

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
                    UnHandleException(e);
                }
            }
        }

        internal void Enqueue(Action callback)
        {
            actions.Enqueue(callback);
        }

        public Action<Exception> OnUnhandledException { internal get; set; } = DefaultUnhandledException;

        private void UnHandleException(Exception e)
        {
            OnUnhandledException?.Invoke(e is WaitableFlowException ? e.InnerException : e);
        }

        private static void DefaultUnhandledException(Exception e)
        {
            Console.Error.WriteLine(e);
        }

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
