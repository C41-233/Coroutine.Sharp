using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Coroutines.Await;
using Coroutines.Base;

// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable PossibleMultipleEnumeration
namespace Coroutines
{
    public class CoroutineManager
    {

        /// <summary>
        /// 调用不带参数版本的StartCoroutine时，针对冒泡异常的处理。仅针对yield return版本的Coroutine有效。
        /// </summary>
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

            #region yield return
            public IWaitable<T> StartCoroutine<T>(
                IEnumerable co, BubbleExceptionApproach bubbleExceptionApproach,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                Assert.NotNull(co, nameof(co));

                var coroutine = new Coroutine<T>(this, co, bubbleExceptionApproach, new DebugInfo
                {
                    Name = name,
                    Method = method,
                    File = file,
                    Line = line,
                });
                return Add(coroutine);
            }

            public IWaitable<T> StartCoroutine<T>(
                IEnumerable co,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0)
            {
                return StartCoroutine<T>(co, CoroutineManager.DefaultBubbleExceptionApproach, name, method, file, line);
            }

            public IWaitable StartCoroutine(
                IEnumerable co, BubbleExceptionApproach bubbleExceptionApproach,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                Assert.NotNull(co, nameof(co));

                var coroutine = new Coroutine(this, co.GetEnumerator(), bubbleExceptionApproach, new DebugInfo
                {
                    Name = name,
                    Method = method,
                    File = file,
                    Line = line,
                });
                return Add(coroutine);
            }

            public IWaitable StartCoroutine(
                IEnumerable co,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                return StartCoroutine(co, CoroutineManager.DefaultBubbleExceptionApproach, name, method, file, line);
            }
            #endregion

            #region await
            public IWaitable StartCoroutine(
                Func<IWaitable> co,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co();
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T>(
                Func<T, IWaitable> co, 
                T arg,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2>(
                Func<T1, T2, IWaitable> co,
                T1 arg1,
                T2 arg2,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3>(
                Func<T1, T2, T3, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4>(
                Func<T1, T2, T3, T4, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5>(
                Func<T1, T2, T3, T4, T5, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6>(
                Func<T1, T2, T3, T4, T5, T6, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7>(
                Func<T1, T2, T3, T4, T5, T6, T7, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7, T8>(
                Func<T1, T2, T3, T4, T5, T6, T7, T8, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                T8 arg8,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
                Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                T8 arg8,
                T9 arg9,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
                Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                T8 arg8,
                T9 arg9,
                T10 arg10,
                T11 arg11,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
                Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                T8 arg8,
                T9 arg9,
                T10 arg10,
                T11 arg11,
                T12 arg12,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
                Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                T8 arg8,
                T9 arg9,
                T10 arg10,
                T11 arg11,
                T12 arg12,
                T13 arg13,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
                Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                T8 arg8,
                T9 arg9,
                T10 arg10,
                T11 arg11,
                T12 arg12,
                T13 arg13,
                T14 arg14,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                T8 arg8,
                T9 arg9,
                T10 arg10,
                T11 arg11,
                T12 arg12,
                T13 arg13,
                T14 arg14,
                T15 arg15,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
                return Add(coroutine);
            }

            public IWaitable StartCoroutine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
                Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, IWaitable> co,
                T1 arg1,
                T2 arg2,
                T3 arg3,
                T4 arg4,
                T5 arg5,
                T6 arg6,
                T7 arg7,
                T8 arg8,
                T9 arg9,
                T10 arg10,
                T11 arg11,
                T12 arg12,
                T13 arg13,
                T14 arg14,
                T15 arg15,
                T16 arg16,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
                return Add(coroutine);
            }

            public IWaitable<R> StartCoroutine<R>(
                Func<IWaitable<R>> co,
                string name = null,
                [CallerMemberName] string method = null,
                [CallerFilePath] string file = null,
                [CallerLineNumber] int line = 0
            )
            {
                PushShareData(name, method, file, line);
                var coroutine = co();
                return Add(coroutine);
            }

            public IWaitable<R> StartCoroutine<T, R>(Func<T, IWaitable<R>> co, T arg)
            {
                AwaitShareDataStatic.Share = new AwaitShareData
                {
                    Container = this,
                };
                var coroutine = co(arg);
                return Add(coroutine);
            }
            #endregion

            private IWaitable<T> Add<T>(IWaitable<T> waitable)
            {
                waitables.Add(waitable);
                waitable.Finally(() => waitables.Remove(waitable));
                return waitable;
            }

            private IWaitable Add(IWaitable waitable)
            {
                waitables.Add(waitable);
                waitable.Finally(() => waitables.Remove(waitable));
                return waitable;
            }

            public void ClearAllCoroutines()
            {
                var list = new IWaitable[waitables.Count];
                waitables.CopyTo(list);
                waitables.Clear();
                foreach (var waitable in list)
                {
                    waitable.Abort();
                }
            }

            private void PushShareData(string name, string method, string file, int line)
            {
                AwaitShareDataStatic.Share = new AwaitShareData
                {
                    Container = this,
                    DebugInfo = new DebugInfo
                    {
                        Name = name,
                        File = file,
                        Line = line,
                        Method = method,
                    }
                };
            }

        }

        public Container CreateContainer() => new Container(this);

    }

    /// <summary>
    /// 当前Coroutine正在等待的IWaitable失败时的处理方法，仅针对yield return模式的Coroutine有效
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
