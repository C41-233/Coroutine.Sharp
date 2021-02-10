using System;
using System.Collections.Generic;
using Coroutines.Base;
using Coroutines.Waitables;
using Coroutines.Waitables.Await;

namespace Coroutines
{

    public sealed partial class CoroutineManager
    {

        public sealed class Container
        {

            public CoroutineManager Manager { get; }

            private readonly HashSet<IWaitable> waitables = new HashSet<IWaitable>();

            private readonly SpinLock spin = new SpinLock();

            public int Count => waitables.Count;

            internal Container(CoroutineManager manager)
            {
                Manager = manager;
            }

            private T Add<T>(T waitable) where T : IWaitable
            {
                using (spin.Hold())
                {
                    waitables.Add(waitable);
                }
                waitable.Finally(() =>
                {
                    using (spin.Hold())
                    {
                        waitables.Remove(waitable);
                    }
                });
                return waitable;
            }

            private void PushShareData()
            {
                AwaitShareDataStatic.Share = new AwaitShareData(this);
            }

            // ReSharper disable once MemberCanBeMadeStatic.Local
            private void PopShareData()
            {
                if (AwaitShareDataStatic.Share != null)
                {
                    AwaitShareDataStatic.Share = default;
                    throw WaitableFlowException.NotAsyncMethod();
                }
            }

            public void Clear()
            {
                IWaitable[] list;
                using (spin.Hold())
                {
                    list = new IWaitable[waitables.Count];
                    waitables.CopyTo(list);
                    waitables.Clear();
                }

                foreach (var waitable in list)
                {
                    try
                    {
                        waitable.Abort();
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }
            }

            #region StartCoroutine
            public IWaitable StartCoroutine(
                Func<IWaitable> co
            )
            {
                PushShareData();
                try
                {
                    var coroutine = co();
                    return Add(coroutine);
                }
                finally
                {
                    PopShareData();
                }
            }

            public IWaitable StartCoroutine<T>(
                Func<T, IWaitable> co,
                T t
            )
            {
                PushShareData();
                try
                {
                    var coroutine = co(t);
                    return Add(coroutine);
                }
                finally
                {
                    PopShareData();
                }
            }

            public IWaitable<R> StartCoroutine<R>(
                Func<IWaitable<R>> co
            )
            {
                PushShareData();
                try
                {
                    var coroutine = co();
                    return Add(coroutine);
                }
                finally
                {
                    PopShareData();
                }
            }
            #endregion

        }
    }

}
